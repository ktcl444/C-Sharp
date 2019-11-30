using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            String a =
                "serviceName=MasterData&methodName=UpdateUser&customParams=%5B%22%7B%5C%22metadataId%5C%22%3A%5C%22%7B2bb6201e-12e1-4855-bf45-2d0bce52afe0%7D%5C%22%2C%5C%22metadataEntry%5C%22%3A%5C%22AppForm%5C%22%2C%5C%22keyValue%5C%22%3A%5C%2239c9a1fc-a528-21a4-ebcd-0d4f94feaf57%5C%22%2C%5C%22record%5C%22%3A%7B%5C%22IsLocked%5C%22%3Anull%2C%5C%22Password%5C%22%3A%5C%228fa14cdd754f91cc6554c9e71929cce7%5C%22%2C%5C%22DisabledReason%5C%22%3Anull%2C%5C%22IsUserChangePWD%5C%22%3Anull%2C%5C%22JobNumber%5C%22%3A%5C%22%5C%22%2C%5C%22UserKind%5C%22%3Anull%2C%5C%22Position%5C%22%3Anull%2C%5C%22IsFormalEstablishment%5C%22%3Anull%2C%5C%22IsMobileUser%5C%22%3Anull%2C%5C%22LockTime%5C%22%3Anull%2C%5C%22Email%5C%22%3A%5C%22%5C%22%2C%5C%22Comments%5C%22%3A%5C%22%5C%22%2C%5C%22ParentGUID%5C%22%3A%5C%2211b11db4-e907-4f1f-8835-b9daab6e1f23%5C%22%2C%5C%22OfficePhone%5C%22%3A%5C%22%5C%22%2C%5C%22ADAccount%5C%22%3Anull%2C%5C%22DefaultStation%5C%22%3Anull%2C%5C%22IsAdvanceUser%5C%22%3Anull%2C%5C%22PhotoUrl%5C%22%3Anull%2C%5C%22IsDisabeld%5C%22%3Afalse%2C%5C%22UserName%5C%22%3A%5C%2256565%5C%22%2C%5C%22UserProject%5C%22%3A%5C%22%5BALL%5D%5C%22%2C%5C%22HomePhone%5C%22%3A%5C%22%5C%22%2C%5C%22IsSaler%5C%22%3Anull%2C%5C%22DepartmentGUID%5C%22%3Anull%2C%5C%22PSWModifyTime%5C%22%3Anull%2C%5C%22JobTitle%5C%22%3Anull%2C%5C%22UserGUID%5C%22%3A%5C%2239c9a1fc-a528-21a4-ebcd-0d4f94feaf57%5C%22%2C%5C%22IsAdmin%5C%22%3A%5C%22false%5C%22%2C%5C%22UserCode%5C%22%3A%5C%22555222222%5C%22%2C%5C%22MobilePhone%5C%22%3A%5C%22%5C%22%2C%5C%22BUGUID%5C%22%3A%5C%2211b11db4-e907-4f1f-8835-b9daab6e1f23%5C%22%2C%5C%22UserGUID1%5C%22%3A%5B%5D%7D%2C%5C%22changedRecord%5C%22%3A%7B%5C%22Password%5C%22%3Atrue%2C%5C%22UserGUID1%5C%22%3Atrue%7D%7D%22%5D";
            Console.WriteLine(System.Web.HttpUtility.UrlDecode(a));
            Console.ReadLine();
        }
    }
}
