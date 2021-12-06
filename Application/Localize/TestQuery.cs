using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Localize
{
    public class TestQuery
    {
        public class TestQuery1 : IRequest<string>
        {
            
        }
        
        public class TestQuery1Handler : IRequestHandler<TestQuery1, string>
        {
            private readonly ILocalizerHelper<TestQuery1Handler> _localizerHelper;

            public TestQuery1Handler(ILocalizerHelper<TestQuery1Handler> localizerHelper)
            {
                _localizerHelper = localizerHelper;
            }
            public Task<string> Handle(TestQuery1 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_localizerHelper.GetString("test"));
            }
        }
    }
}