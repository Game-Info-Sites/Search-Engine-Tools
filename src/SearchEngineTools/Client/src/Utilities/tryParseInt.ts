export function tryParseInt(value: unknown, fallback: number): number
{
  const num = Number(value);
  return !isNaN(num) && num > 0 ? num : fallback;
}
