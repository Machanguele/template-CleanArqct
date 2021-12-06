using System.Collections.Generic;
using System.Threading.Tasks;
using API.Localize;
using Application.Interfaces;
using Application.Localize;
using BoldReports.Linq;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace API.Controllers
{
   
    
        public class TestController : BaseController
        {
            
            private readonly ILocalizerHelper<TestController> _localizerHelper;

            public TestController(ILocalizerHelper<TestController> localizerHelper)
            {
                _localizerHelper = localizerHelper;
            }

             [AllowAnonymous]
             [HttpGet]
             public  async Task<ActionResult<string>> Get()
             {

                 return await Mediator.Send(new TestQuery.TestQuery1());
             }
            
            // [AllowAnonymous]
            // [HttpGet]
            // public  List<ComplaintType> GetComplaintTypes()
            // {
            //     var complaintTypes = new List<ComplaintType>();
            //     complaintTypes.Add(new ComplaintType{Id = 1, Description = "Servico"});
            //     complaintTypes.Add(new ComplaintType{Id = 2, Description = "Atendimento"});
            //     complaintTypes.Add(new ComplaintType{Id = 3, Description = "Taxas altas"});
            //     //List
            //     return complaintTypes;
            // }
        }
    }
