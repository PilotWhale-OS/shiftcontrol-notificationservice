FROM node:20-alpine

ARG SCHEMA_URL
  
# Install dependencies
RUN apk add --no-cache curl tar
  
# Install quicktype globally
RUN npm install -g quicktype
  
# Set working directory
WORKDIR /app
  
# Download and extract JSON schemas
RUN curl -L -o json-event-schemas.tar.gz \
${SCHEMA_URL} \
&& mkdir schemas \
&& tar -xzf json-event-schemas.tar.gz -C schemas \
&& rm json-event-schemas.tar.gz
  
# Generate C# classes from all JSON schema files
RUN quicktype \
--lang csharp \
--namespace ShiftControl.Events \
--src schemas \
--src-lang schema \
--out ShiftControl.Events.cs
  
  # Default command
CMD ["ls", "-la", "/app"]
