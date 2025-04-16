using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTestsAutoFixture
    {
        Mock<IRepository<Partner>> _partnersRepositoryMock;
        PartnersController _partnersController;
        IFixture _fixture;

        public SetPartnerPromoCodeLimitAsyncTestsAutoFixture()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>().OmitAutoProperties().Create();

            //_fixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 2)); не дало эффекта

            // Настройка фикстуры для PartnerPromoCodeLimit без циклической ссылки на Partner
            _fixture.Customize<PartnerPromoCodeLimit>(c => c
                    .Without(l => l.Partner)); // помогает только для создания объектов Partner
        }


        /// <summary>
        /// Метод для генерации тестового объекта класса Partner
        /// </summary>
        /// <param name="IsActive"></param>
        /// <param name="setNumberIssuedPromoCodes"></param>
        /// <param name="setCancelDataOfPartnerLimit"></param>
        /// <returns></returns>
        public Partner GetPartner(bool IsActive = true, bool setNumberIssuedPromoCodes = true, bool setCancelDataOfPartnerLimit = false)
        {
            var partnerBuild = _fixture.Build<Partner>()
                                .With(p => p.IsActive, IsActive);

            if (!setNumberIssuedPromoCodes)
                partnerBuild = partnerBuild.Without(p => p.NumberIssuedPromoCodes);

            var limitBuild = _fixture.Build<PartnerPromoCodeLimit>().Without(l => l.Partner);
            
            if (!setCancelDataOfPartnerLimit)
                limitBuild = limitBuild.Without(l => l.CancelDate);

            Partner partner = partnerBuild.With(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>() { limitBuild.Create() }).Create();

            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);

            return partner;
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotFound_ReturnsNotFound(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partnerId))
                                   .ReturnsAsync((Partner)null);

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            //assert
            actionResult.Should().BeAssignableTo<NotFoundResult>();
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotActive_ReturnsBadRequest(SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            var partner = GetPartner(IsActive: false);

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            actionResult.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_PartnerHasLimit_ResettingPromoCodes(SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            Partner partner = GetPartner();

            //act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.Equal(0, partner.NumberIssuedPromoCodes);
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_PartnerHasLimit_DisablingPreviousLimit(SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            Partner partner = GetPartner();
            var limit = partner.PartnerLimits.Single();

            //act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.NotNull(limit.CancelDate);
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_PartnerHasNotLimit_PromoCodeNotResetting(SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            Partner partner = GetPartner(setCancelDataOfPartnerLimit: true);

            //act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.NotEqual(0, partner.NumberIssuedPromoCodes);
        }

        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimit_LimitIsZero_ReturnsBadRequest(SetPartnerPromoCodeLimitRequest request)
        {
            //arrange
            Partner partner = GetPartner();
            request.Limit = 0;

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            actionResult.Should().BeAssignableTo<BadRequestObjectResult>();
        }
    }
}
