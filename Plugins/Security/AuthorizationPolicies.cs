using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageResizer;

namespace ImageResizer.Plugins.Security
{

    public interface IAuthorizationResponse{
        bool AllowRequest{get;}

    }
     class AuthSuccess:IAuthorizationResponse{
         public bool AllowRequest{get{return true;}}
    }
    class AuthFail:IAuthorizationResponse{
         public bool AllowRequest{get{return false;}}
    }
    public interface IAuthorizationPolicy{
        IAuthorizationResponse AuthorizeRequest(IDictionary<string, object> request);
    }
    /// <summary>
    /// //Policy "read" - Allows get or head requests only. Prohibit PUT, PATCH, POST, etc.
    /// </summary>
    public class ReadOnlyPolicy:IAuthorizationPolicy{

         public IAuthorizationResponse AuthorizeRequest(IDictionary<string,object> request)
        {
            
 	        if ("GET".Equals(request["owin.RequestMethod"] as string, StringComparison.OrdinalIgnoreCase) ||
                "HEAD".Equals(request["owin.RequestMethod"] as string, StringComparison.OrdinalIgnoreCase)){
                return new AuthSuccess();
            } else{
                return new AuthFail();
            }
             //var inst = new Instructions(){ Width = 1; Height = 2, Format= "png", Mode = }
             
        }
    }


 

    //Policy "ip" - allow only IP Address, list, or range. Ipv4 and IPv6.
    //Policy "anyquery" - allow any querystring (locks the path, but not the commands)
    //Policy "expires" - allow responses until expiration
    //Policy "allowvalue" - allow width/height/format/quality values to vary if they are on the whitelist.

        //ri-policies = comma delimited list; (optional) Ex. expires, read, ip, anyquery
        //ri-expires = seconds since epoch (when access terminates) (optional, must be used with policy)
        //ri-whitelisted-ips=
        //ri-allowedvalues-[key]=comma delimited list of URL-encoded values permitted to [key]

        //sorted querystring = querystring + ri-policies + ri-keyid -> lowercase keys -> sort
        //canoncializedresource = path + sorted querystring  ri-policies
        //ri-signature = urlb64(key id) + "|" + base64u(hmacsha256(secret, hmacsha256(UTF8(canoncializedresource))))
        //http://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html
        //http://stackoverflow.com/questions/23605869/any-holes-in-securing-a-http-request-with-hmac-using-only-the-http-method-and-ur
        //http://s3.amazonaws.com/doc/s3-developer-guide/RESTAuthentication.html
        //http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/PrivateContent.html
    


    //Policy "ip" - allow only IP Address, list, or range. Ipv4 and IPv6.
    //Policy "anyquery" - allow any querystring (locks the path, but not the commands)
    //Policy "expires" - allow responses until expiration
    //Policy "allowvalue" - allow width/height/format/quality values to vary if they are on the whitelist.

}
