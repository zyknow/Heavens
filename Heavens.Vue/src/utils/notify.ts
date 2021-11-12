import { RequestResult } from 'src/api/_typing'
import { LooseDictionary, Notify } from 'quasar'
import { Component } from 'vue'

/**
 * Notify 参数配置,参考Quasar Notify 提供的参数
 */
export type NotifyOption = {
  /**
   * Optional type (that has been previously registered) or one of the out of the box ones ('positive', 'negative', 'warning', 'info', 'ongoing')
   */
  type?: string
  /**
   * Color name for component from the Quasar Color Palette
   */
  color?: string
  /**
   * Color name for component from the Quasar Color Palette
   */
  textColor?: string
  /**
   * The content of your message
   */
  message?: string
  /**
   * The content of your optional caption
   */
  caption?: string
  /**
   * Render message as HTML; This can lead to XSS attacks, so make sure that you sanitize the message first
   */
  html?: boolean
  /**
   * Icon name following Quasar convention; Make sure you have the icon library installed unless you are using 'img:' prefix
   */
  icon?: string
  /**
   * URL to an avatar/image; Suggestion: use statics folder
   */
  avatar?: string
  /**
   * Useful for notifications that are updated; Displays a Quasar spinner instead of an avatar or icon; If value is Boolean 'true' then the default QSpinner is shown
   */
  spinner?: boolean | Component
  /**
   * Window side/corner to stick to
   * Default value: bottom
   */
  position?:
    | 'top-left'
    | 'top-right'
    | 'bottom-left'
    | 'bottom-right'
    | 'top'
    | 'bottom'
    | 'left'
    | 'right'
    | 'center'
  /**
   * Override the auto generated group with custom one; Grouped notifications cannot be updated; String or number value inform this is part of a specific group, regardless of its options; When a new notification is triggered with same group name, it replaces the old one and shows a badge with how many times the notification was triggered
   * Default value: (message + caption + multiline + actions labels + position)
   */
  group?: boolean | string | number
  /**
   * Color name for the badge from the Quasar Color Palette
   */
  badgeColor?: string
  /**
   * Color name for the badge text from the Quasar Color Palette
   */
  badgeTextColor?: string
  /**
   * Notification corner to stick badge to; If notification is on the left side then default is top-right otherwise it is top-left
   * Default value: (top-left/top-right)
   */
  badgePosition?: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right'
  /**
   * Style definitions to be attributed to the badge
   */
  badgeStyle?: any[] | string | LooseDictionary
  /**
   * Class definitions to be attributed to the badge
   */
  badgeClass?: any[] | string | LooseDictionary
  /**
   * Show progress bar to detail when notification will disappear automatically (unless timeout is 0)
   */
  progress?: boolean
  /**
   * Class definitions to be attributed to the progress bar
   */
  progressClass?: any[] | string | LooseDictionary
  /**
   * Add CSS class(es) to the notification for easier customization
   */
  classes?: string
  /**
   * Key-value for attributes to be set on the notification
   */
  attrs?: LooseDictionary
  /**
   * Amount of time to display (in milliseconds)
   * Default value: 5000
   */
  timeout?: number
  /**
   * Notification actions (buttons); If a 'handler' is specified or not, clicking/tapping on the button will also close the notification; Also check 'closeBtn' convenience prop
   */
  actions?: any[]
  /**
   * Function to call when notification gets dismissed
   */
  onDismiss?: () => void
  /**
   * Convenience way to add a dismiss button with a specific label, without using the 'actions' prop; If set to true, it uses a label accordding to the current Quasar language
   */
  closeBtn?: boolean | string
  /**
   * Put notification into multi-line mode; If this prop isn't used and more than one 'action' is specified then notification goes into multi-line mode by default
   */
  multiLine?: boolean
  /**
   * Ignore the default configuration (set by setDefaults()) for this instance only
   */
  ignoreDefaults?: boolean
}

/**
 * 在App.Vue 中 初始化该 useI18n t ,否则在其他ts文件中使用notify会报错
 */
export const notifyI18n = {
  t(msg: string): string {
    return ''
  },
}

/**
 * 该接口会把提示信息转换成 I18n
 */
export type INotify = {
  info(msg: string, opt?: NotifyOption): void
  error(msg: string, opt?: NotifyOption): void
  warn(msg: string, opt?: NotifyOption): void
  success(msg: string, opt?: NotifyOption): void
  /**
   * axios执行结果提示
   * @param res
   * @param opt ?: NotifyOption
   */
  response(res: RequestResult, opt?: NotifyOption)
  /**
   * axios执行消息提示，仅当执行失败时
   * @param res
   * @param opt ?: NotifyOption
   */
  responseOnErr(res: RequestResult, opt?: NotifyOption)
}

/**
 * notify基础设置
 * @param msg
 * @returns
 */
const baseNotify = (msg: string): NotifyOption => {
  return {
    message: notifyI18n.t(msg),
    position: 'top',
    group: false,
    timeout: 2500,
    color: 'white',
    classes: 'notify-caption-text', // 用于自定义样式
  }
}

const responseActionSuccess = '执行成功'
const responseActionFalid = '执行失败'

/**
 * 该接口会把提示信息转换成 I18n
 */
export const notify: INotify = {
  info(msg: string, opt?: NotifyOption): void {
    const options: NotifyOption = {
      icon: 'info',
      textColor: 'blue-10',
    }
    Notify.create({ ...baseNotify(msg), ...options, ...opt })
  },
  error(msg: string, opt?: NotifyOption): void {
    const options: NotifyOption = {
      icon: 'cancel',
      textColor: 'red-9',
    }
    Notify.create({ ...baseNotify(msg), ...options, ...opt })
  },
  warn(msg: string, opt?: NotifyOption): void {
    const options: NotifyOption = {
      icon: 'warning',
      textColor: 'orange-9',
    }
    Notify.create({ ...baseNotify(msg), ...options, ...opt })
  },
  success(msg: string, opt?: NotifyOption): void {
    const options: NotifyOption = {
      icon: 'check_circle',
      textColor: 'positive',
    }
    Notify.create({ ...baseNotify(msg), ...options, ...opt })
  },
  response(res: RequestResult, opt?: NotifyOption): void {
    if (!res || !res.succeeded) {
      // notify.error(res.errors?.toString() || responseActionFalid, opt)
      notify.error(
        typeof res.errors == 'string'
          ? res.errors
          : JSON.stringify(res.errors) || responseActionFalid,
        opt,
      )
    } else {
      notify.success(responseActionSuccess, opt)
    }
  },
  responseOnErr(res: RequestResult, opt?: NotifyOption): void {
    if (!res || !res.succeeded) notify.response(res, opt)
  },
}
