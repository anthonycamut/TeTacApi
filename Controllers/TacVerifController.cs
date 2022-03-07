using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventSource;
using TeTacApi.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Security.Authentication;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.AspNetCore.Cors;

namespace TeTacApi.Controllers
{  
  [Route("[controller]")]
  [ApiController]
  [Authorize]
  public class TeTacApiController : ControllerBase
  {
    private readonly TeTacApiContextSalon _context;
    private readonly ILogger _logger;
    private readonly string tokenApiPreProd = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJTZndld3BDV0VaUG5RLVhXWlJPVTBXajFfemZxREVObTh2RFVWZmZWQ0JvIn0.eyJleHAiOjE2NTIyODI1ODQsImlhdCI6MTY0NDMzMzc4NCwianRpIjoiOTk5NzM0YzMtMWM5Ny00ODBhLTg4NGMtM2RlNDVkMDFhNzY1IiwiaXNzIjoiaHR0cHM6Ly9hdXRoLm1lc3NlcnZpY2VzLXBwLmluZ3JvdXBlLmNvbS9hdXRoL3JlYWxtcy9QSU5HIiwic3ViIjoiMDIwNjEyY2YtN2U0MC00MzFiLWIwMGYtYjMwY2I3YWYwZDdhIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoidGFjdi1jbGllbnQtYXBpLWxpdGUiLCJzZXNzaW9uX3N0YXRlIjoiZmQzMTRhZWQtMjZhZi00OGRlLTkyMWQtN2Y4NDAxMzZjNTYxIiwiYWNyIjoiMSIsInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsIlJPTEVfVkVSSUZZX0NPTlRST0xfQ09WSURfTElURSJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InRhY3YtY2xpZW50LWFwaS1saXRlIjp7InJvbGVzIjpbInVtYV9wcm90ZWN0aW9uIl19fSwic2NvcGUiOiJlbWFpbCBwcm9maWxlIG9mZmxpbmVfYWNjZXNzIiwiY2xpZW50SWQiOiJ0YWN2LWNsaWVudC1hcGktbGl0ZSIsImNsaWVudEhvc3QiOiIxMC4xMC4yLjI0OCIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwic2lyZW4iOiIzNTI5NzM2MjIiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJzZXJ2aWNlLWFjY291bnQtdGFjdi1jbGllbnQtYXBpLWxpdGUiLCJzZXJ2aWNlcyI6InRhY3YiLCJjbGllbnRBZGRyZXNzIjoiMTAuMTAuMi4yNDgifQ.g4nIgXn9dby7BEa4jR1wXTmLqzcvHSTWlK1Wli4ZE2_9HH1yicIwfM0ow4yr_vLoi2d0FnW-VzsObCfujrZhUeX7Yy0yQRXYd7L6-fuSKLSWf3HBhWbtgCWYWdd4Y0sNl9J4-XaAzHE4TYRcYtVTe-5fSj6RcPHE4AnLFVji-VpX4iyGjgSbwq5OMv6PTepAemjQy2wMDDtEiM0fcYg8YnRlm_y-LdZeUCNhFyzZGN9GmBmC30h5I-um43_kFX__saJzekNLloIYs3oSfdIZfkXolddnfRLrnPMWnh8h7Pgez0cG79g7mbzvMpciyWOVRNi6P9K-zRAa3n1pSSKhkg";
    private readonly string tokenApiProd = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJTZndld3BDV0VaUG5RLVhXWlJPVTBXajFfemZxREVObTh2RFVWZmZWQ0JvIn0.eyJleHAiOjE2NTIyODI1ODQsImlhdCI6MTY0NDMzMzc4NCwianRpIjoiOTk5NzM0YzMtMWM5Ny00ODBhLTg4NGMtM2RlNDVkMDFhNzY1IiwiaXNzIjoiaHR0cHM6Ly9hdXRoLm1lc3NlcnZpY2VzLXBwLmluZ3JvdXBlLmNvbS9hdXRoL3JlYWxtcy9QSU5HIiwic3ViIjoiMDIwNjEyY2YtN2U0MC00MzFiLWIwMGYtYjMwY2I3YWYwZDdhIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoidGFjdi1jbGllbnQtYXBpLWxpdGUiLCJzZXNzaW9uX3N0YXRlIjoiZmQzMTRhZWQtMjZhZi00OGRlLTkyMWQtN2Y4NDAxMzZjNTYxIiwiYWNyIjoiMSIsInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsIlJPTEVfVkVSSUZZX0NPTlRST0xfQ09WSURfTElURSJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InRhY3YtY2xpZW50LWFwaS1saXRlIjp7InJvbGVzIjpbInVtYV9wcm90ZWN0aW9uIl19fSwic2NvcGUiOiJlbWFpbCBwcm9maWxlIG9mZmxpbmVfYWNjZXNzIiwiY2xpZW50SWQiOiJ0YWN2LWNsaWVudC1hcGktbGl0ZSIsImNsaWVudEhvc3QiOiIxMC4xMC4yLjI0OCIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwic2lyZW4iOiIzNTI5NzM2MjIiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJzZXJ2aWNlLWFjY291bnQtdGFjdi1jbGllbnQtYXBpLWxpdGUiLCJzZXJ2aWNlcyI6InRhY3YiLCJjbGllbnRBZGRyZXNzIjoiMTAuMTAuMi4yNDgifQ.g4nIgXn9dby7BEa4jR1wXTmLqzcvHSTWlK1Wli4ZE2_9HH1yicIwfM0ow4yr_vLoi2d0FnW-VzsObCfujrZhUeX7Yy0yQRXYd7L6-fuSKLSWf3HBhWbtgCWYWdd4Y0sNl9J4-XaAzHE4TYRcYtVTe-5fSj6RcPHE4AnLFVji-VpX4iyGjgSbwq5OMv6PTepAemjQy2wMDDtEiM0fcYg8YnRlm_y-LdZeUCNhFyzZGN9GmBmC30h5I-um43_kFX__saJzekNLloIYs3oSfdIZfkXolddnfRLrnPMWnh8h7Pgez0cG79g7mbzvMpciyWOVRNi6P9K-zRAa3n1pSSKhkg";
    private readonly string urlPreProd = "https://api.ppd.tacv.myservices-ingroupe.com";
    private readonly string urlProd = "https://api.tacv.myservices-ingroupe.com";
    private readonly string action2DOC = "/api/document/2D-DOC";
    private readonly string actionDDC = "/api/document/DCC";

    private static HttpClient client;
     


    public TeTacApiController(TeTacApiContextSalon context, ILogger<TeTacApiController> logger)
    {
      _context = context;
      _logger = logger; 

      HttpClientHandler clientHandler = new HttpClientHandler();
      clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

      clientHandler.SslProtocols = SslProtocols.Tls12; 
      client = new HttpClient(clientHandler); 

    }
    private void Log(string _txt)
    {
            
                _logger.LogInformation("IP : " + HttpContext.Connection.RemoteIpAddress.ToString() + " | " + _txt + " | " + " user : " + User.FindFirst("fullName").Value);                            
     }

        [HttpPost("TacVerif")]    
    public async Task<ActionResult<string>> TacVerifProd(string qrcodecontent)
    {
      return await TacVerif(qrcodecontent);
    }

    [HttpOptions("TacVerifPreProd")]
    public async Task<ActionResult<string>> optionTacVerifPreProd(string qrcodecontent)
    {      
      return await TacVerif(qrcodecontent, false);      
    }

   

    [HttpPost("TacVerifPreProd")]
    
    public async Task<ActionResult<string>>  TacVerifPreProd(string qrcodecontent)
    {
      return await TacVerif(qrcodecontent, false);
    }

    private async Task<ActionResult<string>> TacVerif(string qrcodecontent, Boolean prod=true)
    {
      var token = tokenApiProd;
      var url = urlProd;
            var action = "";
       if(!prod)
      {
        token = tokenApiPreProd;
        url = urlPreProd;
      }
      
      if(qrcodecontent.StartsWith("DC0") && (qrcodecontent.Substring(20, 2) == "B2" || qrcodecontent.Substring(20, 2) == "L1")) // on détermine si c'est un 2D DOC si ça commence par DC0 et si le Code d’identification du type de document est B2 (TestCOVID) ou L1 (VaccinCOVID)
      {
                url += action2DOC;
                
            }
      else if(true) // sinon on considère que c'est un DCC -> peut être à affiner, Le message DCC doit être de type DCC Test, DCC Recovery ou DCC Vaccin (Cf norme DCC Européen).
      {
                url += actionDDC;
                
            }
      Log("Call TacVerif with url : " + url + " - token : " + token);


      var request = new HttpRequestMessage(HttpMethod.Post,
      url);
      request.Headers.Add("Authorization", "Bearer " + token);
      request.Content = new StringContent(qrcodecontent);

      var response = await client.SendAsync(request);

      if(response.IsSuccessStatusCode)
      {

        string apiResponse = response.Content.ReadAsStringAsync().Result;
        Log("Success response with  api Response : " + apiResponse);
        return apiResponse;
      }
      else
      {
        Log("Error response with statucode " + response.StatusCode.ToString());
        return "";
      }


    }


  }
}