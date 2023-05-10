using System;  // 包含通用 C# 庫
using System.Collections.Generic;  // 包含集合型別相關的 C# 庫
using System.Linq;  // 包含 LINQ 相關的 C# 庫
using System.Reflection;  // 包含反射相關的 C# 庫
using System.Text.Json;
using System.Threading.Tasks;  // 包含多執行緒相關的 C# 庫
using com.emecca.model;  // 引入 com.emecca.model 命名空間，用來引用模型
using com.emecca.service;  // 引入 com.emecca.service 命名空間，用來引用服務
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;  // 引入 ASP.NET Core 的 MVC 框架
using Newtonsoft.Json;  // 引入 Newtonsoft.Json 庫，用來處理 JSON 格式
using Json = System.Text.Json.JsonSerializer;

namespace com.emecca.controller
{
    [ApiController]  // 告訴 ASP.NET Core 這是一個 API 控制器
    [Route("[controller]")]  // 設定路由模板
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmeccaAutoController : ControllerBase
    {
        // 建構子，用來注入 IServiceProvider
        private readonly IServiceProvider _serviceProvider;

        public EmeccaAutoController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        // HTTP GET 請求 service 及 method都是透過別名來抓取
        [HttpGet("{service}/{method}")]
        public IActionResult Get(string service, string method, [FromQuery] Dictionary<string, string> parameters)
        {
            List<object> args = new List<object>();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    // 檢查參數類型並轉換為特定格式
                    
                   if (DateTime.TryParse(param.Value, out DateTime datetimeValue))
                   {
                        args.Add(datetimeValue);
                   }
                   else if (param.Value is string)
                   {
                        args.Add(param.Value);
                   }
                   else
                   {
                        args.Add(param.Value);
                   }                
                }
            }

            return CallMethod(service, method, args);
        }
        // HTTP POST 請求 service 及 method都是透過別名來抓取
        [HttpPost("{service}/{method}")]
        public IActionResult Post(string service, string method, [FromBody] object? parametersObject)
        {
            System.Console.WriteLine("parameters");
            System.Console.WriteLine($"Received parameters object type: {parametersObject.GetType()}");
            System.Console.WriteLine($"Received parameters object value: {parametersObject}");

            List<object> parameters = new List<object>();

            if (parametersObject is Dictionary<string, string> stringDict)
            {
                foreach (var item in stringDict)
                {
                    parameters.Add(item.Value);
                }
            }
            else if (parametersObject is Dictionary<string, object> objectDict)
            {
                foreach (var item in objectDict)
                {
                    parameters.Add(item.Value);
                }
            }
            else if (parametersObject is List<object> objectList)
            {
                parameters = objectList;
            }
            // Add support for JsonElement
            else if (parametersObject is JsonElement jsonElement)
            {
                //var dictionary = Json.Deserialize<Dictionary<string, string>>(jsonElement.GetRawText());
                //parameters = dictionary.Values.Cast<object>().ToList();
                parameters.Add((object)parametersObject);
            }
            else
            {
                return BadRequest("Invalid parameters format.");
            }


            if (parameters == null)
            {
                parameters = new List<object>();
            }
            else
            {
                foreach (var param in parameters)
                {
                    System.Console.WriteLine($"Value: {param}");
                }
            }

            return CallMethod(service, method, parameters);
        }
        // 呼叫指定的方法
        private IActionResult CallMethod(string serviceName, string methodName, List<object>? parameters)
        {
            if (parameters == null)
            {
                parameters = new List<object>();
            }
            System.Console.WriteLine(serviceName.ToString()+"    methodNmae:"+methodName);        
            // 取得指定的服務
            var service = _serviceProvider.GetService(GetServiceType(serviceName));
            if (service == null)
            {
                return NotFound($"Service {serviceName} not found.");
            }
            System.Console.WriteLine(service.ToString());
            // 取得指定的方法
            MethodInfo method = GetMethodFromAlias(service, methodName);
            if (method == null)
            {
                return NotFound($"Method {methodName} not found in service {serviceName}.");
            }

            try
            {
                var a = parameters.ToArray();
                // 執行指定的方法，取得回傳值
                /**
                 動態方法調用：在 CallMethod 方法中，使用了動態方法調用該代碼根據傳入的方法名 method 
                   和對應的服務 service，使用反射機制調用了相應的方法。由於在 parameters 
                   中傳入的是一個 List<object>，所以需要使用 ToArray() 方法將其轉換為一個 object 陣列，
                   以便作為方法的參數傳入。
                   **/
                var result = method.Invoke(service, parameters.ToArray<object>());

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // private Type GetTypeFromName(string typeName)
        // {
        //     var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        //     return type;
        // }
        /**
        經由Service的別名去找出服務
        **/
        private Type GetServiceType(string typeName)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var serviceAlias = type.GetCustomAttribute<ServiceAliasAttribute>();
                if (serviceAlias != null && serviceAlias.Alias.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                {
                    return type;
                }
            }
            var typefromname = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

            return typefromname;
        }
        /**
        經由Method的別名去找出方法
        **/
        private MethodInfo GetMethodFromAlias(object service, string methodName)
        {
            // var methods = service.GetType().GetMethods();
            // foreach (var m in methods)
            // {
            //     var attribute = m.GetCustomAttribute<MethodAliasAttribute>();
            //     if (attribute != null)
            //     {
            //         System.Console.WriteLine(attribute.Alias);
            //     }
            //     else
            //     {
            //         System.Console.WriteLine("NULLABLE");
            //     }
            // }
            var methodInfo = service.GetType().GetMethods()
        .FirstOrDefault(m => GetMethodAlias(m) == methodName);

            return methodInfo;

            // var methods = service.GetType().GetMethods();
            // System.Console.WriteLine("methodName:"+methodName.ToString());
            // foreach (var method in methods)
            // {
            //     var aliasAttr = method.GetCustomAttribute<MethodAliasAttribute>();
            //     System.Console.WriteLine("akuasAttr:"+aliasAttr.Alias.ToString());
            //     if (aliasAttr != null && aliasAttr.Alias == methodName)
            //     {
            //         return method;
            //     }
            // }

            // var typefromname = service.GetType().GetMethod(methodName);
            // System.Console.WriteLine("typefromname:"+typefromname.ToString());
            // return typefromname;
        }
        /**
        經由Method的別名去找出方法
        **/
        private string GetMethodAlias(MethodInfo methodInfo)
        {
            // Get the alias of the method
            return methodInfo.GetCustomAttribute<MethodAliasAttribute>()?.Alias ?? methodInfo.Name;
        }

    }
}
