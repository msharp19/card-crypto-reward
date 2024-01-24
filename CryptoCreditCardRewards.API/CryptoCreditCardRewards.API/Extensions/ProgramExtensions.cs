using AspNetCoreRateLimit;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CryptoCreditCardRewards.API.Filters;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.API.Mapping;
using CryptoCreditCardRewards.API.Services.Hosted;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models.Events;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Api;
using CryptoCreditCardRewards.Services.Api.Interfaces;
using CryptoCreditCardRewards.Services.API;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Services.Http;
using CryptoCreditCardRewards.Services.Http.Interfaces;
using CryptoCreditCardRewards.Services.Misc;
using CryptoCreditCardRewards.Services.Misc.Interfaces;
using CryptoCreditCardRewards.Services.EventHandlers;
using CryptoCreditCardRewards.Services.Helpers.Interfaces;
using CryptoCreditCardRewards.Services.Helpers;

namespace CryptoCreditCardRewards.API.Extensions
{
    public static class ProgramExtensions
    {
        /// <summary>
        /// Add CORS to API (restrict front-end access)
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors();

            return builder;
        }

        /// <summary>
        /// Add application settings + environment variables - environment specific
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureSettings(this WebApplicationBuilder builder)
        {
            // Base app settings is loaded
            builder.Configuration.AddJsonFile("appsettings.json", false, true);

            // Environment dependant settings
            if (builder.Environment.IsDevelopment())
                builder.Configuration.AddJsonFile("appsettings.Development.json", false, true);
            if (builder.Environment.IsProduction())
                builder.Configuration.AddJsonFile("appsettings.Production.json", false, true);

            // Add environemnts vars
            builder.Configuration.AddEnvironmentVariables();

            return builder;
        }

        /// <summary>
        /// Handler for bad request responses
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureMVC(this WebApplicationBuilder builder)
        {
            // Add Mvc and custom options
            builder.Services.AddMvcCore()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        return ModelStateValidationErrorHelper.BuildBadRequest(actionContext.ModelState);
                    };
                }).AddApiExplorer();

            return builder;
        }

        /// <summary>
        /// Setup the webserver for linux & windows server
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureWebServer(this WebApplicationBuilder builder)
        {
            // Ensure there is no limit for file upload size
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
                options.AllowSynchronousIO = true;
            });
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = null;
                options.AllowSynchronousIO = true;
            });

            // For large file support
            builder.Services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            return builder;
        }

        /// <summary>
        /// Add rate limiting to the API
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureRateLimiting(this WebApplicationBuilder builder)
        {
            // For rate limiting
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimitSettings"));
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // inject counter and rules stores
            builder.Services.AddInMemoryRateLimiting();

            return builder;
        }

        /// <summary>
        /// Configure service level http clients
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureHttpClients(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();

            // Add HTTP clients
            builder.Services.AddHttpClient("CreditCardTransactionService", (client) => { client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CreditCardTransactionServiceSettings:Endpoint")); });
            builder.Services.AddHttpClient("CryptoCompareService", (client) => { client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CryptoCompareSettings:Endpoint")); });

            return builder;
        }

        /// <summary>
        /// Add Options from configuration to API
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureOptions(this WebApplicationBuilder builder)
        {
            // Add Options
            builder.Services.Configure<IdentityOptions>(options => { });
            builder.Services.Configure<JwtSettings>(options => builder.Configuration.GetSection("JwtSettings").Bind(options));
            builder.Services.Configure<CacheSettings>(options => builder.Configuration.GetSection("CacheSettings").Bind(options));
            builder.Services.Configure<EmailSettings>(options => builder.Configuration.GetSection("EmailSettings").Bind(options));
            builder.Services.Configure<CryptoCompareSettings>(options => builder.Configuration.GetSection("CryptoCompareSettings").Bind(options));
            builder.Services.Configure<GetBlockSettings>(options => builder.Configuration.GetSection("GetBlockSettings").Bind(options));

            builder.Services.Configure<StakingSettings>(options => builder.Configuration.GetSection("StakingSettings").Bind(options));
            builder.Services.Configure<WalletAddressSettings>(options => builder.Configuration.GetSection("WalletAddressSettings").Bind(options));
            builder.Services.Configure<SystemWalletAddressSettings>(options => builder.Configuration.GetSection("SystemWalletAddressSettings").Bind(options));

            builder.Services.Configure<RewardInstructionIssuerHostedServiceSettings>(options => builder.Configuration.GetSection("RewardInstructionIssuerHostedServiceSettings").Bind(options));
            builder.Services.Configure<RewardPaymentInstructionHostedServiceSettings>(options => builder.Configuration.GetSection("RewardPaymentInstructionHostedServiceSettings").Bind(options));
            builder.Services.Configure<StakingDepositInstructionHostedServiceSettings>(options => builder.Configuration.GetSection("StakingDepositInstructionHostedServiceSettings").Bind(options));
            builder.Services.Configure<StakingWithdrawalInstructionHostedServiceSettings>(options => builder.Configuration.GetSection("StakingWithdrawalInstructionHostedServiceSettings").Bind(options));
            builder.Services.Configure<TransactionConfirmationHostedServiceSettings>(options => builder.Configuration.GetSection("TransactionConfirmationHostedServiceSettings").Bind(options));
            builder.Services.Configure<WithdrawalInstructionHostedServiceSettings>(options => builder.Configuration.GetSection("WithdrawalInstructionHostedServiceSettings").Bind(options));
            builder.Services.Configure<RewardPaymentInstructionIssuerHostedServiceSettings>(options => builder.Configuration.GetSection("RewardPaymentInstructionIssuerHostedServiceSettings").Bind(options));     

            return builder;
        }

        /// <summary>
        /// Add the db contexts (EF Core setup)
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureDbContexts(this WebApplicationBuilder builder)
        {
            builder.Services.SetContext<CryptoCreditCardRewardsDbContext>(builder.Configuration.GetConnectionString("ApplicationDatabase"));

            return builder;
        }

        /// <summary>
        /// Configure the services for the application (add to DI)
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            // Setup the data services
            builder.Services.AddTransient<ICacheService, MemoryCacheService>();
            builder.Services.AddTransient<IWalletAddressService, WalletAddressService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRewardSelectionService, UserRewardSelectionService>();
            builder.Services.AddTransient<IInstructionService, InstructionService>();
            builder.Services.AddTransient<ICryptoCurrencyService, CryptoCurrencyService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<ISystemWalletAddressService, SystemWalletAddressService>();
            builder.Services.AddTransient<IWhitelistAddressService, WhitelistAddressService>();
            builder.Services.AddTransient<ICryptoRewardBandsService, CryptoRewardBandsService>();

            //builder.Services.AddTransient<IBlockchainService, EthereumService>();
            //builder.Services.AddTransient<IBlockchainService, BitcoinService>();
            builder.Services.AddTransient<IBlockchainProviderFactory, BlockchainProviderFactory>();
            builder.Services.AddTransient<IConversionProviderFactory, ConversionProviderFactory>();

            // Setup the Http services
            builder.Services.AddTransient<ICreditCardTransactionService, CreditCardTransactionService>(s =>
                new CreditCardTransactionService(s.GetService<IHttpClientFactory>().CreateClient("CreditCardTransactionService"))
            );
            builder.Services.AddTransient<CryptoCompareService>(s =>
                 new CryptoCompareService(s.GetService<IHttpClientFactory>().CreateClient("CryptoCompareService"), s.GetService<ILogger<CryptoCompareService>>(), s.GetService<IOptions<CryptoCompareSettings>>())
             );

            // Setup the API level services
            builder.Services.AddTransient<IUserCreateService, UserCreateService>();
            builder.Services.AddTransient<IUserRewardSelectionService, UserRewardSelectionService>();
            builder.Services.AddTransient<IUserUpdateService, UserUpdateService>();
            builder.Services.AddTransient<IWalletCreateService, WalletCreateService>();
            builder.Services.AddTransient<IStakingCreateService, StakingCreateService>();
            builder.Services.AddTransient<ICryptoCurrencyCreateService, CryptoCurrencyCreateService>();
            builder.Services.AddTransient<ICryptoCurrencyUpdateService, CryptoCurrencyUpdateService>();
            builder.Services.AddTransient<ICryptoWithdrawalService, CryptoWithdrawalService>();
            builder.Services.AddTransient<ITransactionCreateService, TransactionCreateService>();
            builder.Services.AddTransient<ICryptoRewardBandCreateService, CryptoRewardBandCreateService>();
            builder.Services.AddTransient<ISystemWalletAddressCreateService, SystemWalletAddressCreateService>();
            builder.Services.AddTransient<IWhitelistAddressCreateService, WhitelistAddressCreateService>();
            builder.Services.AddTransient<ICryptoRewardBandDeleteService, CryptoRewardBandDeleteService>();
            builder.Services.AddTransient<IWhitelistAddressUpdateService, WhitelistAddressUpdateService>();
            builder.Services.AddTransient<IStakingDeleteService, StakingDeleteService>();
            builder.Services.AddTransient<IUserRewardSelectionUpdateService, UserRewardSelectionUpdateService>();
            builder.Services.AddTransient<ITransactionUpdateService, TransactionUpdateService>();

            builder.Services.AddTransient<IStakingService, StakingService>();

            builder.Services.AddTransient<ICreditCardRewardIssuanceService, CreditCardRewardIssuanceService>();
            builder.Services.AddTransient<IRewardPaymentInstructionProcessingService, RewardPaymentInstructionProcessingService>();
            builder.Services.AddTransient<IStakingDepositInstructionProcessorService, StakingDepositInstructionProcessorService>();
            builder.Services.AddTransient<IStakingWithdrawalInstructionProcessorService, StakingWithdrawalInstructionProcessorService>();
            builder.Services.AddTransient<ITransactionConfirmationService, TransactionConfirmationService>();
            builder.Services.AddTransient<ICreditCardRewardIssuanceService, CreditCardRewardIssuanceService>();
            builder.Services.AddTransient<IMonthlyRewardInstructionIssuerService, MonthlyRewardInstructionIssuerService>();
            builder.Services.AddTransient<IWithdrawalInstructionProcessorService, WithdrawalInstructionProcessorService>();

            builder.Services.AddScoped<INotificationHandler<CreatedWalletAddressEvent>, WalletAddressCreatedHandler>();
            builder.Services.AddScoped<INotificationHandler<CreatedRewardInstructionEvent>, CreatedRewardInstructionHandler>();

            // Misc
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            builder.Services.AddScoped<ModelStateValidationFilter>();

            // Setup Mediatr service
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            return builder;
        }

        /// <summary>
        /// Configure the Hanfire delayed execution framework
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureHangfire(this WebApplicationBuilder builder)
        {
            // Hangfire queues
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("ApplicationDatabase")));

            // Only uncomment if it is required to process events on this server (currently should be limited to events api)
            if (builder.Configuration.GetValue<bool>("HangfireSettings:ServerEnabled"))
            {
                builder.Services.AddHangfireServer(options =>
                {
                    options.WorkerCount = builder.Configuration.GetValue<int>("HangfireSettings:WorkerCount");
                });

                // Stop failed jobs from being rerun automatically. This enforces manual intervention in the point of failure
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            }

            return builder;
        }

        /// <summary>
        /// Configure API level automapper for response mapping (add to DI)
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureAutomapper(this WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(CryptoCreditCardRewardsMappingProfile).Assembly);

            return builder;
        }

        /// <summary>
        /// Add background jobs to API
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureHostedServices(this WebApplicationBuilder builder)
        {
            // Hosted services
            if (builder.Configuration.GetValue<bool>("HostedServiceOptions:Enabled"))
            {
                builder.Services.AddHostedService<MonthlyRewardInstructionIssuerHostedService>();
                builder.Services.AddHostedService<RewardPaymentInstructionHostedService>();
                builder.Services.AddHostedService<StakingWithdrawalInstructionHostedService>();
                builder.Services.AddHostedService<StakingDepositInstructionHostedService>();
                builder.Services.AddHostedService<WithdrawalInstructionHostedService>();
                builder.Services.AddHostedService<TransactionConfirmationHostedService>();
                builder.Services.AddHostedService<RewardPaymentInstructionIssuerHostedService>();
            }

            return builder;
        }

        /// <summary>
        /// Configure the authentication for the API
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            // Get JWT options
            var jwtOptions = new JwtSettings();
            builder.Configuration.GetSection("JwtSettings").Bind(jwtOptions);

            // Get OpenID Options
            var openIdOptions = new OpenIdSettings();
            builder.Configuration.GetSection("OpenIdOptions").Bind(openIdOptions);

            // Get JWT signing keys
            var signingKeys = SigningKeyHelper.GetSigningKeysAsync(openIdOptions.WellKnownEndpoints).GetAwaiter().GetResult();

            // Setup JWT Auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // JWT Signing Key
                    IssuerSigningKey = SigningKeyHelper.CreateSigningKey(jwtOptions.JwtSigningKey),

                    // OIDC Signing Keys
                    IssuerSigningKeys = signingKeys,

                    // Name claim definition
                    NameClaimType = ClaimTypes.NameIdentifier,

                    // JWT Signing Key
                    ValidAudience = jwtOptions.Audience,

                    // External Audiences valid in JWT (to expect from)
                    ValidAudiences = jwtOptions.ValidAudiences,

                    // JWT Issuer
                    ValidIssuer = jwtOptions.Issuer,

                    // External Issuers valid in JWT (to expect from)
                    ValidIssuers = jwtOptions.ValidIssuers,

                    // Ensure issuer is validated
                    ValidateIssuer = true,

                    // Ensure audience is validated
                    ValidateAudience = true,

                    // Require check for expiration
                    RequireExpirationTime = true,

                    // So expriy works
                    ClockSkew = TimeSpan.Zero,

                    // Custom issuer validater to support multi tenant requests
                    IssuerValidator = (issuer, securityToken, validationParameters) => IssuerHelper.ValidateIssuer(issuer, securityToken, validationParameters),
                };

                // Add events for adding claims that OpenID cannot (ie. role)
                options.Events = JwtBearerEventHelper.CreateJwtBearerEvents();
            });

            // Setup Authorization for role base access
            builder.Services.AddAuthorizationPolicies(builder.Configuration);

            return builder;
        }

        /// <summary>
        /// Configure the API documentation
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
        {
            // Add Swagger docs
            builder.Services.AddSwaggerGen(c =>
            {
                var version = builder.Configuration.GetValue<string>("Swagger:Version");

                c.SwaggerDoc(version, new OpenApiInfo { Title = builder.Configuration.GetValue<string>("Swagger:Title"), Version = version });
                c.CustomSchemaIds(x => x.FullName);
                c.DocumentFilter<LowercaseDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return builder;
        }

        /// <summary>
        /// Add the controllers to the API (endpoints)
        /// </summary>
        /// <param name="builder">The api builder</param>
        /// <returns>The api builder</returns>
        public static WebApplicationBuilder ConfigureControllers(this WebApplicationBuilder builder)
        {
            // Setup controller
            builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
            })
            .AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return builder;
        }
    }
}
