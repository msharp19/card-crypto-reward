using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CryptoCreditCardRewards.Utilities
{
    public static class JsonExtensions
    {
        [DbFunction("JSON_VALUE", Schema = "", IsBuiltIn = true)]
        public static string JsonValue(string column, [NotParameterized] string path)
        {
            throw new NotSupportedException();
        }
    }
}