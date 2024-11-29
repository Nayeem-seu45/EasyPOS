import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DateUtilService {
  // Convert a date string from one format to another
  convertDateFormat(dateString: string, fromFormat: string, toFormat: string): string | null {
    const [day, month, year] = this.parseDateString(dateString, fromFormat);

    if (day && month && year) {
      const targetDate = new Date(year, month - 1, day); // Create Date object (month is 0-based)
      return this.formatDate(targetDate, toFormat);
    }

    return null;
  }

  // Parse a date string into its components based on the given format
  private parseDateString(dateString: string, format: string): [number, number, number] {
    const formatParts = format.split(/[-/]/); // Split format into parts
    const dateParts = dateString.split(/[-/]/); // Split date string into parts

    const day = parseInt(dateParts[formatParts.indexOf('dd')], 10);
    const month = parseInt(dateParts[formatParts.indexOf('MM')], 10);
    const year = parseInt(dateParts[formatParts.indexOf('yyyy')], 10);

    return [day, month, year];
  }

  // Format a Date object into a string in the desired format
  private formatDate(date: Date, format: string): string {
    const dd = String(date.getDate()).padStart(2, '0'); // Day with leading zero
    const MM = String(date.getMonth() + 1).padStart(2, '0'); // Month with leading zero
    const yyyy = date.getFullYear(); // Full year

    return format
      .replace('dd', dd)
      .replace('MM', MM)
      .replace('yyyy', String(yyyy));
  }

  // Parse a string into a Date object (handles only `yyyy/MM/dd` format directly)
  parseToDate(dateString: string, format: string): Date | null {
    if (format === 'yyyy/MM/dd') {
      const [year, month, day] = dateString.split('/').map((x) => parseInt(x, 10));
      return new Date(year, month - 1, day); // Create Date object (month is 0-based)
    }

    console.error('Unsupported format for parsing to Date:', format);
    return null;
  }

   /**
 * Converts a date string from one format to another.
 *
 * @param {string} date - The date string to be converted.
 * @param {'dmy' | 'mdy' | 'ymd'} [inputFormat='dmy'] - The format of the input date string.
 *        - 'dmy': day/month/year (default)
 *        - 'mdy': month/day/year
 *        - 'ymd': year/month/day
 * @param {'ymd' | 'dmy' | 'mdy'} [outputFormat='ymd'] - The desired format of the output date string.
 *        - 'ymd': year-month-day (default)
 *        - 'dmy': day-month-year
 *        - 'mdy': month-day-year
 * @param {string} [inputSplitter='/'] - The character that splits the input date parts. Default is '/'.
 * @param {string} [outputSplitter='-'] - The character that will split the output date parts. Default is '-'.
 * @returns {string} The converted date string.
 * @throws {Error} If the input date format is invalid or the date string is not in the expected format.
 *
 * @example
 * // Convert from day/month/year to year-month-day
 * convertDate('06/11/2024', 'dmy', 'ymd'); // Output: '2024-11-06'
 *
 * @example
 * // Convert from month-day-year to day/month/year with different splitters
 * convertDate('11-06-2024', 'mdy', 'dmy', '-', '/'); // Output: '06/11/2024'
 */
 convertDateStringFormat(
  date: string,
  inputFormat: 'dmy' | 'mdy' | 'ymd' = 'dmy',
  outputFormat: 'ymd' | 'dmy' | 'mdy' = 'ymd',
  inputSplitter: string = '/',
  outputSplitter: string = '-'
): string {
  if (!date) {
    return '';
  }

  const dateParts = date.split(inputSplitter);
  if (dateParts.length !== 3) {
    throw new Error('Invalid date format');
  }

  let day: string, month: string, year: string;

  switch (inputFormat) {
    case 'dmy':
      [day, month, year] = dateParts;
      break;
    case 'mdy':
      [month, day, year] = dateParts;
      break;
    case 'ymd':
      [year, month, day] = dateParts;
      break;
    default:
      throw new Error('Invalid input date format');
  }

  let result: string;
  switch (outputFormat) {
    case 'ymd':
      result = `${year}${outputSplitter}${month}${outputSplitter}${day}`;
      break;
    case 'dmy':
      result = `${day}${outputSplitter}${month}${outputSplitter}${year}`;
      break;
    case 'mdy':
      result = `${month}${outputSplitter}${day}${outputSplitter}${year}`;
      break;
    default:
      throw new Error('Invalid output date format');
  }

  return result;
}

/**
 * Converts a date string from 'day/month/year' format to 'year-month-day' format.
 *
 * This function is a convenience wrapper around the `convertDateStringFormat` function.
 * It transforms a date string from the 'dmy' format with '/' as the input splitter
 * to the 'ymd' format with '-' as the output splitter.
 *
 * @param {string} dateString - The date string to be converted. It should be in the 'day/month/year' format.
 * @returns {string} - The converted date string in the 'year-month-day' format.
 *                    Returns an empty string if the input date string is invalid or empty.
 *
 * @example
 * // Convert '06/11/2024' from 'day/month/year' to 'year-month-day'
 * convertToDateOnlyFormat('06/11/2024'); // Output: '2024-11-06'
 */
convertToDateOnlyFormat(dateString: string): string {
  return this.convertDateStringFormat(dateString, 'dmy', 'ymd', '/', '-');
}

convertTo_dmy_Format(dateString: string): string {
  return this.convertDateStringFormat(dateString, 'ymd', 'dmy', '-', '/');
}

 isValidDateStringFormat(dateString: string): boolean {
  if (!dateString) {
    return false;
  }

  let splitter: string;
  if (dateString.includes('/')) {
    splitter = '/';
  } else if (dateString.includes('-')) {
    splitter = '-';
  } else {
    return false;
  }

  const dateParts = dateString.split(splitter);
  if (dateParts.length !== 3) {
    return false;
  }

  const [day, month, year] = dateParts;
  return this.isNumber(day) && this.isNumber(month) && this.isNumber(year);
}

private isNumber(value: string): boolean {
  return !isNaN(Number(value));
}

  

  //  convert_dmy_to_ymd(dmy: string, splitter: string = '/'): string | null {
  //   // Check if the input is a valid date format
  //   const dateParts = dmy.split(splitter);
    
  //   if (dateParts.length !== 3) {
  //     console.error('Invalid date format. Expected format is dd/MM/yyyy.');
  //     return null;  // Return null if the format is incorrect
  //   }
  
  //   const [day, month, year] = dateParts;
    
  //   // Validate the parts
  //   if (isNaN(Number(day)) || isNaN(Number(month)) || isNaN(Number(year))) {
  //     console.error('Invalid date parts. All parts must be numbers.');
  //     return null;
  //   }
  
  //   // Optional: Check for valid date ranges (like 32/13/2024, 02/30/2024 etc.)
  //   const date = new Date(Number(year), Number(month) - 1, Number(day));
  //   if (date.getDate() !== Number(day) || date.getMonth() + 1 !== Number(month) || date.getFullYear() !== Number(year)) {
  //     console.error('Invalid date: Date does not exist.');
  //     return null;
  //   }
  
  //   // Return the formatted date string in yyyy/MM/dd format
  //   return `${year}${splitter}${month}${splitter}${day}`;
  // }
  
}
