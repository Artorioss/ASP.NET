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
    public class SetPartnerPromoCodeLimitAsyncTests
    {

        Mock<IRepository<Partner>> _partnersRepositoryMock;
        PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests() 
        {
            _partnersRepositoryMock = new Mock<IRepository<Partner>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
        }

        public Partner CreateBasePartner()
        {
            var partner = new Partner()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                NumberIssuedPromoCodes = 20,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100,
                        CancelDate = new DateTime(2020, 09, 9)
                    }
                }
            };

            return partner;
        }

        public SetPartnerPromoCodeLimitRequest CreateRequest() 
        {
            var request = new SetPartnerPromoCodeLimitRequest() 
            {
                EndDate = DateTime.Now.AddDays(5),
                Limit = 100
            };

            return request;
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotFound_ReturnsNotFound()
        {
            //arrange
            Guid guidId = Guid.NewGuid();
            SetPartnerPromoCodeLimitRequest request = new SetPartnerPromoCodeLimitRequest();

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(guidId, request);

            //assert
            actionResult.Should().BeAssignableTo<NotFoundResult>();
        }


        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotActive_ReturnsBadRequest()
        {
            //arrange
            Partner partner = CreateBasePartner();
            partner.IsActive = false;
            
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, null);

            //assert
            actionResult.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerHasLimit_ResettingPromoCodes() 
        {
            //arrange
            Partner partner = CreateBasePartner();
            partner.PartnerLimits.Single().CancelDate = null;
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);

            var request = CreateRequest();

            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.Equal(0, partner.NumberIssuedPromoCodes);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerHasLimit_DisablingPreviousLimit()
        {
            //arrange
            Partner partner = CreateBasePartner();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);
            var limit = partner.PartnerLimits.Single();
            limit.CancelDate = null;
            var request = CreateRequest();
            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.NotNull(limit.CancelDate);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerHasNotLimit_PromoCodeNotResetting() 
        {
            //arrange
            Partner partner = CreateBasePartner();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);
            var request = CreateRequest();
            
            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            Assert.NotEqual(0, partner.NumberIssuedPromoCodes);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_LimitIsZero_ReturnsBadRequest() 
        {
            //arrange
            Partner partner = CreateBasePartner();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id))
                                   .ReturnsAsync(partner);
            var request = CreateRequest();
            request.Limit = 0;
            //act
            var actionResult = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //assert
            actionResult.Should().BeAssignableTo<BadRequestObjectResult>();
        }

    }
}