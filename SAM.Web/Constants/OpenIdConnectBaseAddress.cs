using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Constants
{
    public static class OpenIdConnectBaseAddress
    {
        public const string BaseAddress = "(endereço de uri onde ser realizará a consulta)";

        public const string AuthorizeEndpoint = BaseAddress + "/connect/authorize";
        public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
        public const string TokenEndpoint = BaseAddress + "/connect/token";
        public const string UserInfoEndpoint = BaseAddress + "/connect/userinfo";
        public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";

        public const string AspNetWebApiSampleApi = BaseAddress;
        public const string AspNetWebApiSampleApiUsingPoP = BaseAddress;

    }
}