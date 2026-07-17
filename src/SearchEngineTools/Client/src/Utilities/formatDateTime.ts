const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'medium',
  timeStyle: 'short',
});

export function formatDateTime(dateTime: string | undefined): string
{
  if (!dateTime) { return ''; }
  const parsedValue = new Date(dateTime);
  if (isNaN(parsedValue.getTime())) { return ''; }
  return dateTimeFormatter.format(parsedValue);
}
