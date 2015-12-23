using System.Collections.Generic;
using Nancy.Security;

namespace DemoNancy
{
    public class User : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; private set; }

        public static IUserIdentity Anonymous { get; private set; }

        static User()
        {
            Anonymous = new User { UserName = "anyonymous" };
        }
    }
}