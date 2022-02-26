export const enumToOption = (enumType: any) => {
  const option = [] as any[]
  for (const [key, val] of Object.entries(enumType)) {
    option.push({
      label: key,
      value: val
    } as any)
  }
  return option
}
