const express = require('express')
const request = require('request')
// parse urlencoded request bodies into req.body
const bodyParser = require('body-parser')
const app = express()
const port = 3000

app.use(bodyParser.urlencoded({ extended: false }))
app.use(bodyParser.json())

app.get('/', (req, res) =>
{
    console.log(req);
    res.statusCode = 200;
    res.setHeader('Content-Type', 'text/plain');
    res.end('Hello World');
})

app.get('/subs/', (req, res) => {
    console.log(req);
    res.statusCode = 200;
    res.setHeader('Content-Type', 'text/plain');
    res.end('AWS SNS endpoint only');
})

app.post('/subs/', (req, res) => {
  let body = ''

  req.on('data', (chunk) => {
    body += chunk.toString()
  })

  req.on('end', () => {
    let payload = JSON.parse(body)

    if (payload.Type === 'SubscriptionConfirmation') {
      const promise = new Promise((resolve, reject) => {
        const url = payload.SubscribeURL

        request(url, (error, response) => {
          if (!error && response.statusCode == 200) {
            console.log('Yes! We have accepted the confirmation from AWS')
            return resolve()
          } else {
            return reject()
          }
        })
      })

      promise.then(() => {
        res.end("ok")
      })
    }

    if (payload.Type === 'Notification') {
        const msg = payload.Message;
        console.log(msg);
        res.statusCode = 200;
        res.end()
      }


    if (payload.Type === 'UnsubscribeConfirmation') {
        console.log('Unsubscribed')
        res.statusCode = 200;
        res.end()
      }
  })
})

app.post('/subs-raw/', (req, res) => {
    let body = ''
  
    req.on('data', (chunk) => {
      body += chunk.toString()
    })
  
    req.on('end', () => {
      if(body.charAt(0) == '<') {
        console.log(body);
        res.statusCode = 200;
        res.end()
      } 
      else 
      {
      
        let payload = JSON.parse(body)
    
        if (payload.Type === 'SubscriptionConfirmation') {
            const promise = new Promise((resolve, reject) => {
            const url = payload.SubscribeURL
    
            request(url, (error, response) => {
                if (!error && response.statusCode == 200) {
                console.log('Yes! We have accepted the raw confirmation from AWS')
                return resolve()
                } else {
                return reject()
                }
            })
            })
    
            promise.then(() => {
            res.end("ok")
            })
        }
    
        if (payload.Type === 'Notification') {
            console.log('RAW \n' + payload)

            const msg = payload.Message;
            console.log(msg);
            res.statusCode = 200;
            res.end()
            }
    
    
        if (payload.Type === 'UnsubscribeConfirmation') {
            console.log('Unsubscribed')
            res.statusCode = 200;
            res.end()
            }
        }
    })
  })
  

app.listen(port, () => console.log('Example app listening on port ' + port + '!'))