import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'spacedComma',
})
export class SpacedCommaPipe implements PipeTransform {
  transform(value: string): string {
    return value
      .split(',')
      .map((word) => word.trim())
      .join(', ');
  }
}
