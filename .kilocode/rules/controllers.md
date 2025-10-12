# Controllers

Every controller should follow this rules:

- Use primary constructors when possible
- Use records for requests and responses
- Use Data Validation Annotations to validate requests
- Validate first before processing the request
- Complex logic (i.e. database acess) should be done in services
