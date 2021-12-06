using Application.Interfaces;
using Microsoft.Extensions.Localization;

namespace Application.Helpers
{
    public class LocalizerHelper <T>: ILocalizerHelper<T> where T: class
    {
        private readonly IStringLocalizer<T> _localizer;

        public LocalizerHelper(IStringLocalizer<T> localizer)
        {
            _localizer = localizer;
        }
        public string GetString(string key, string param = "")
        {
            
            var data = _localizer[key, ""].Value;
            return data;
        }
    }
}