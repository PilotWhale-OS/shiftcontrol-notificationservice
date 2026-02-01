FROM node:20-alpine
  
# Install dependencies
RUN apk add --no-cache curl tar
  
# Install quicktype globally
RUN npm install -g quicktype
  
# Set working directory
WORKDIR /app
  
# Copy generator script
COPY script-events.sh .
  
# Run script
CMD ["sh", "script-events.sh"]
