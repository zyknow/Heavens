import moment from 'moment'

//  YYYY-MM-DD HH:mm:ss
export const dateFormat = (dataStr: string | number, pattern = 'YYYY-MM-DD') => {
  return moment(dataStr).format(pattern)
}

export const dateFormatNow = (dataStr: string | number) => {
  return moment(dataStr).fromNow()
}
