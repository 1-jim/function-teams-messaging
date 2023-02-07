# SendMessageToTeams

* Create a webhook in your teams channel
* Add the Url to the Application Setting "WEBHOOK_URL"
* Deploy to Azure Function App
* Post a message to the endpoint with a payload

```json
{
    "title": "My Title",
    "message": "My Message"
}
```

Enjoy