# aws-subscriber-http-endpoint-cs-js
Dual implementations of the correct response to subscribe an HTTP endpoint to an AWS SNS Topic in Node/js and AspNetCore/dotnet5

This code represents the response to a subscription request HTTP call from AWS SNS when Subscriptions are created for a SNS Topic. There is the implementation in /subs/ for the json payload with Message properts and the implementation in /subs-raw/ for the raw message in Post Body with no wrapping json.

Reference to [aws documentation](https://docs.aws.amazon.com/sns/latest/dg/SendMessageToHttp.prepare.html)

To run these on a local machine you must make sure to forward the ports from your router to your local machine. Also make sure that the ip you use is setup for your machine: 

- dotnet : in appsettings.json key Urls
- nodejs : not necessary

I used port 3000 for node and 3001 for dotnet so as to run both subscriptions simultaneously to check for arrival on both from AWS. 

## run dotnet with 
```
cd cs
dotnet run
```

## run node with 
```
cd js
npm install express
npm install request
npm install body-parser
node app.js
```

You can then create a subscription in AWS SNS and the services will be called. They will auto confirm the subscriptions and will be ready to recieve the messages and display them on the command line.



As described by aws you should [validate any notification](https://docs.aws.amazon.com/sns/latest/dg/sns-verify-signature-of-message.html). [Java example](https://docs.aws.amazon.com/sns/latest/dg/sns-example-code-endpoint-java-servlet.html)
