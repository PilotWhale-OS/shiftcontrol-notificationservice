curl -L -o openapi.json $SCHEMA_URL

nswag openapi2csclient \
    /input:openapi.json \
    /output:/out/ShiftserviceClient.cs \
    /namespace:NotificationService.ShiftserviceClient