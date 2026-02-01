curl -L -o json-event-schemas.tar.gz $SCHEMA_URL
mkdir schemas
tar -xzf json-event-schemas.tar.gz -C schemas
rm json-event-schemas.tar.gz

quicktype \
  --lang csharp \
  --namespace ShiftControl.Events \
  --src schemas \
  --src-lang schema \
  --out /out/EventClasses.cs