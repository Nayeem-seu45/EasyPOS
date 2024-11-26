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

  convert_dmy_to_ymd(dmy: string, splitter: string = '/'){
    if(!dmy){
      return '';
    }
    const dateParts = dmy.split(splitter);
    return `${dateParts[2]}${splitter}${dateParts[1]}${splitter}${dateParts[0]}`;
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
