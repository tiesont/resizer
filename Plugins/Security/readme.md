The RequestSigning plugin has two parts:

1. URL Signing. Given a path, query, and authorization policies, it can produce a signed URL.

2. All incoming requests are verified against their included signature and policies.



 

    //Policy "ip" - allow only IP Address, list, or range. Ipv4 and IPv6.
    //Policy "anyquery" - allow any querystring (locks the path, but not the commands)
    //Policy "expires" - allow responses until expiration
    //Policy "allowvalue" - allow width/height/format/quality values to vary if they are on the whitelist.

        //ri-policies = comma delimited list; (optional) Ex. expires, read, ip, anyquery
        //ri-expires = seconds since epoch (when access terminates) (optional, must be used with policy)
        //ri-allowed-ips=
        //ri-allowed-[key]=comma delimited list of URL-encoded values permitted to [key]

        //sorted querystring = querystring + ri-policies + ri-keyid -> lowercase keys -> sort
        //canoncializedresource = path + sorted querystring  ri-policies
        //ri-signature = urlb64(key id) + "|" + base64u(hmacsha256(secret, hmacsha256(UTF8(canoncializedresource))))
        //http://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html
        //http://stackoverflow.com/questions/23605869/any-holes-in-securing-a-http-request-with-hmac-using-only-the-http-method-and-ur
        //http://s3.amazonaws.com/doc/s3-developer-guide/RESTAuthentication.html
        //http://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/PrivateContent.html
    


