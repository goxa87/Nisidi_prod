/**
 * Форматирование даты к виду dd.MM.yy - чч:мм:сс
 * @param {any} value
 */
export function DateFromObjectToDateTime(value) {
    console.log('v1',value)
    if (!value || value == null || value == 0) return '-';
    value = new Date(value);
    return `${value.getDate() < 10 ? '0' : ''}${value.getDate()}.${value.getMonth()+1<10 ? '0' : ''}${value.getMonth() + 1}.${value.getFullYear()} ${value.getHours()}:${value.getMinutes()}:${value.getSeconds()}`;
}