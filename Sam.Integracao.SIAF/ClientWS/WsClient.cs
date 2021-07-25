using System;
using System.Net;                       //NetworkCredential,WebClient
using System.Security.Permissions;      //SecurityPermissionAttribute
using System.CodeDom.Compiler;          //CodeDomProvider,CompilerParameters,CompilerResults
using System.Reflection;                //MethodInfo
using System.Net.Security;
using smDescription = System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Web.Services.Description;
using System.Xml.Serialization;
using System.CodeDom;
using Sam.Common.Util;
using Sam.Integracao.SIAF.Configuracao;

namespace Sam.Integracao.SIAF.ClientWS
{
    public class WsClient
    {
        readonly string proxyIIS = ConfiguracoesSIAF.enderecoProxy;
        const string nomeServico = "RecebeMSG";
        const string nomeMetodo = "Mensagem";


        [SecurityPermission(SecurityAction.InheritanceDemand, Unrestricted = true)]
        private object wrapperCallWebService(string webServiceAsmxUrl, string serviceName, string methodName, object[] args, string proxyIIS = null)
        {
            try
            {
                return this.callWebService(webServiceAsmxUrl, serviceName, methodName, args, proxyIIS);
            }
            catch (Exception excErroExecucao)
            {
                return excErroExecucao.Message;
            }
        }

        /// <summary>
        /// Método utilizado para ignorar validação de certificado SSL
        /// </summary>
        private static void initiateSSLTrust()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        }

        private object callWebService(string webServiceAsmxUrl, string serviceName, string methodName, object[] args, string proxyIIS = null)
        {
            object retVal = null;
            object wsInstanceClass = null;
            string sufixoConsulta = null;
            string wsdlUrl = null;
            MethodInfo methodInfo = null;


            sufixoConsulta = "?wsdl";
            wsdlUrl = String.Format("{0}{1}", webServiceAsmxUrl, sufixoConsulta);
            try
            {
                var assemblyWS = getWebServiceClientAssembly(wsdlUrl, proxyIIS);

                // Finally, Invoke the web service method
                wsInstanceClass = assemblyWS.CreateInstance(serviceName);
                if (wsInstanceClass.IsNotNull())
                    methodInfo = wsInstanceClass.GetType().GetMethod(methodName);

                if (methodInfo.IsNotNull())
                    retVal = methodInfo.Invoke(wsInstanceClass, args);
            }
            catch (Exception excCompilacaoOnTheFly)
            {
                throw new Exception(excCompilacaoOnTheFly.Message);
            }

            return retVal;
        }

        /// <summary>
        /// Metodo para gerar a DLL do WCF Client on-the-fly (similar to 'Add Service Reference...')
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Assembly createWCFClientAssembly(string url, WebProxy proxyIIS)
        {
            Assembly assembly = null;


            Uri address = new Uri(url);
            smDescription.MetadataExchangeClientMode mexMode = smDescription.MetadataExchangeClientMode.HttpGet;
            smDescription.MetadataExchangeClient metadataExchangeClient = new smDescription.MetadataExchangeClient(address, mexMode);
            metadataExchangeClient.HttpCredentials = proxyIIS.Credentials;
            metadataExchangeClient.ResolveMetadataReferences = true;

            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);


            smDescription.MetadataSet metadataSet = metadataExchangeClient.GetMetadata();
            smDescription.WsdlImporter wsdlImporter = new smDescription.WsdlImporter(metadataSet);
            Collection<smDescription.ContractDescription> contracts = wsdlImporter.ImportAllContracts();
            smDescription.ServiceEndpointCollection allEndpoints = wsdlImporter.ImportAllEndpoints();

            smDescription.ServiceContractGenerator serviceContractGenerator = new smDescription.ServiceContractGenerator();
            foreach (smDescription.ContractDescription contract in contracts)
                serviceContractGenerator.GenerateServiceContractType(contract);

            // Generate a code file for the contracts.
            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions();
            codeGeneratorOptions.BracingStyle = "C";

            // Create Compiler instance of a specified language.
            CompilerResults compilerResults;
            using (CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp"))
            {

                // Adding WCF-related assemblies references as copiler parameters, so as to do the compilation of particular service contract.
                CompilerParameters compilerParameters = new CompilerParameters(new string[] { "System.dll", "System.ServiceModel.dll", "System.Runtime.Serialization.dll" });

                compilerParameters.OutputAssembly = "wsclient";
                compilerParameters.GenerateInMemory = true;
                compilerParameters.WarningLevel = 1;

                compilerResults = codeDomProvider.CompileAssemblyFromDom(compilerParameters, serviceContractGenerator.TargetCompileUnit);
            }

            if (compilerResults.Errors.Count <= 0)
            {
                assembly = compilerResults.CompiledAssembly;
            }
            else
            {
                foreach (CompilerError error in compilerResults.Errors)
                    Console.WriteLine(error.ErrorNumber + ": " + error.ErrorText + " " + error.IsWarning + " " + error.Line);

                throw new Exception("Compiler Errors - unable to build Web Service Assembly");
            }


            return assembly;
        }

        /// <summary>
        /// Metodo para gerar a DLL do WCF Client on-the-fly (similar to 'Add Reference...')
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //private Assembly createWSClientAssembly(string webServiceAsmxUrl, string proxyIIS)
        private Assembly createWSClientAssembly(string webServiceAsmxUrl)
        {
            Assembly assembly = null;
            CompilerParameters compilerParameters = null;
            CompilerResults compilerResults = null;
            CodeGeneratorOptions codeGeneratorOptions = null;
            CodeNamespace codeNamespace = null;
            CodeCompileUnit codeCompileUnit = null;
            TempFileCollection tempFileCollection = null;
            string[] assemblyReferences = null;


            WebClient wbClient = new WebClient();

            initiateSSLTrust();
            //if (webServiceAsmxUrl.Contains("(url de homologação)") || webServiceAsmxUrl.ToLowerInvariant().Contains(@"/localhost") || !webServiceAsmxUrl.Contains("www6.fazenda.sp.gov.br"))
            if (webServiceAsmxUrl.ToLowerInvariant().Contains(Constante.CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO) || webServiceAsmxUrl.ToLowerInvariant().Contains(@"/localhost") || !webServiceAsmxUrl.Contains(Constante.CST_URL_SEFAZ_AMBIENTE_PRODUCAO))
                wbClient.UseDefaultCredentials = true;
            //else if (webServiceAsmxUrl.Contains("www6.fazenda.sp.gov.br"))
            else if (webServiceAsmxUrl.ToLowerInvariant().Contains(Constante.CST_URL_SEFAZ_AMBIENTE_PRODUCAO))
                wbClient.Proxy = new WebProxy(proxyIIS, true); //IIS Proxy.


            // Connect To the web service
            Stream stream = wbClient.OpenRead(webServiceAsmxUrl);
            // Now read the WSDL file describing a service.
            ServiceDescription description = ServiceDescription.Read(stream, true);

            ///// LOAD THE DOM /////////
            // Initialize a service description importer.
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            importer.ProtocolName = "Soap12"; // Use SOAP 1.2.
            importer.AddServiceDescription(description, null, null);

            // Generate a proxy client.
            importer.Style = ServiceDescriptionImportStyle.Client;

            // Generate properties to represent primitive values.
            importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;



            // Initialize a Code-DOM tree into which we will import the service.
            codeNamespace = new CodeNamespace();
            codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(codeNamespace);

            // Import the service into the Code-DOM tree. This creates proxy code that uses the service.
            ServiceDescriptionImportWarnings sdiWarning = importer.Import(codeNamespace, codeCompileUnit);
            if (sdiWarning == 0) // If zero then we are good to go
            {
                tempFileCollection = new TempFileCollection(obterDiretorioExecucaoSistema(), false);
                codeGeneratorOptions = new CodeGeneratorOptions() { BracingStyle = "C", };
                assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };
                // Create Compiler instance of a specified language.
                using (CodeDomProvider codeDOMProvider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    compilerParameters = new CompilerParameters(assemblyReferences, "wsclient", true);

                    compilerParameters.OutputAssembly = String.Format(@"{0}\{1}", obterDiretorioExecucaoSistema(), "wsclient.DLL");
                    compilerParameters.CompilerOptions = "/optimize";
                    compilerParameters.GenerateInMemory = true;
                    compilerParameters.WarningLevel = 1;

                    compilerResults = codeDOMProvider.CompileAssemblyFromDom(compilerParameters, codeCompileUnit);

                    // Check For Errors
                    if (compilerResults.Errors.Count > 0)
                    {
                        foreach (CompilerError error in compilerResults.Errors)
                            Console.WriteLine(error.ErrorNumber + ": " + error.ErrorText + " " + error.IsWarning + " " + error.Line);

                        throw new Exception("Compiler Errors - unable to build Web Service Assembly");
                    }
                    else if (compilerResults.Errors.Count <= 0)
                    {
                        assembly = compilerResults.CompiledAssembly;
                    }
                }
            }
            return assembly;
        }

        internal static string obterDiretorioExecucaoSistema()
        {
            string fullPathDir = null;
            AppDomain currentDomain = null;
            

            currentDomain = AppDomain.CurrentDomain;
            fullPathDir = currentDomain.SetupInformation.PrivateBinPath;

            return fullPathDir;
        }

        private Tuple<bool, Assembly> existeWSClientAssembly(string url, string proxyIIS)
        {
            bool assemblyFoiCarregado = false;
            string fullPathFile = null;
            Assembly assembly = null;
            Assembly[] assems = null;


            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                //Provide the current application domain evidence for the assembly.
                //Evidence asEvidence = currentDomain.Evidence;
                //Load the assembly from the application directory using a simple name.

                //Create an assembly called CustomLibrary to run this sample.
                fullPathFile = String.Format("{0}{1}", obterDiretorioExecucaoSistema(), @"\wsclient.dll"); 
                if (File.Exists(fullPathFile))
                {
                    currentDomain.Load("wsclient.dll");

                    //Make an array for the list of assemblies.
                    assems = currentDomain.GetAssemblies();

                    //List the assemblies in the current application domain.
                    if (assems.HasElements())
                        //assembly = assems.Where(assName => assName.Location.Contains("wsclient")).FirstOrDefault();
                        assembly = assems.Where(assName => assName.EscapedCodeBase == fullPathFile).FirstOrDefault();
                        //if (assems.Count(assName => assName.EscapedCodeBase == fullPathFile) == 1)
                            //assembly = assems.Where(assName => assName.EscapedCodeBase == fullPathFile).FirstOrDefault();
                }
            }
            catch (FileNotFoundException)
            {
                //throw new Exception("Erro ao carregar assembly WSClient existente (arquivo não encontrado).");
                //assembly = createWSClientAssembly(url, proxyIIS);
                assembly = createWSClientAssembly(url);
            }
            catch (NotSupportedException)
            {
                //throw new Exception("Erro ao carregar assembly WSClient existente (método/função não suportado/a).");
                //assembly = createWSClientAssembly(url, proxyIIS);
                assembly = createWSClientAssembly(url);
            }
            catch (Exception excErroGenerico)
            {
                throw new Exception(String.Format("Erro ao carregar assembly WSClient existente (Descricao: {0}).", excErroGenerico.Message));
            }

            assemblyFoiCarregado = (assembly != null);

            return new Tuple<bool, Assembly>(assemblyFoiCarregado, assembly);
        }

        /// <summary>
        /// Metodo que verifica se a DLL jah foi gerada e existe no diretorio, senao gera a mesma em memoria
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Assembly getWebServiceClientAssembly(string url, string proxyIIS)
        {
            Assembly assembly = null;

            try
            {
                var consultaExistenciaAssemblyWS = existeWSClientAssembly(url, proxyIIS);

                if (consultaExistenciaAssemblyWS.Item1)
                    assembly = consultaExistenciaAssemblyWS.Item2;
                else
                    //assembly = createWSClientAssembly(url, proxyIIS);
                    assembly = createWSClientAssembly(url);
            }
            catch (Exception excErroRuntime)
            {
                throw new Exception("Erro ao gerar assembly WSClient on-the-fly", excErroRuntime);
            }

            return assembly;
        }

        public string Mensagem(string loginUsuarioSIAF, string senhaUsuarioSIAF, string anoBaseConsulta, string ugeGestora, string msgEstimulo, bool isConsulta)
        {
            string strRetorno = null;
            string urlConsulta = null;
            string proxyIIS = null;
            string[] argsWS = null;


            proxyIIS = ConfiguracoesSIAF.enderecoProxy;
            urlConsulta = ConfiguracoesSIAF.wsURLEnvio;
            if (isConsulta)
            {
                ugeGestora = string.Empty;
                urlConsulta = ConfiguracoesSIAF.wsURLConsulta;
            }


            argsWS = new string[] { loginUsuarioSIAF, senhaUsuarioSIAF, anoBaseConsulta, ugeGestora, msgEstimulo };
            strRetorno = wrapperCallWebService(urlConsulta, nomeServico, nomeMetodo, argsWS, proxyIIS) as string;


            return strRetorno;
        }
    }
}