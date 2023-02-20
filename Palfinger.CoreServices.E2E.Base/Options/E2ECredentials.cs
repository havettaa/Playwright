using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Palfinger.CoreServices.E2E.Base.Options
{
    public sealed record E2ECredentials
    {
        [Required]
        public required string UsernameGeneralAgent { get; init; }
        [Required]
        public required string PasswordGeneralAgent { get; init; }


        public required string UsernameEndCustomerAdmin { get; init; }
        public required string UsernameEndCustomerUserManager { get; init; }
        public required string UsernameEndCustomerCompanyManager { get; init; }
        public required string UsernamePartnerNetworkAreaManager { get; init; }
        public required string UsernamePartnerNetworkManager { get; init; }
        public required string UsernamePartnerNetworkReader { get; init; }
        public required string UsernameSupplierManager { get; init; }

    }
}
