import {Component, Input, Optional, Self} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ControlValueAccessor, FormsModule, NgControl} from '@angular/forms';
import {InputNumberModule} from 'primeng/inputnumber';

@Component({
  selector: 'app-number-field',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputNumberModule
  ],
  templateUrl: './number-field.component.html',
  styleUrl: './number-field.component.css'
})
export class NumberFieldComponent implements ControlValueAccessor {
  @Input() id: string = `number-field-${Math.random().toString(36).substring(2, 9)}`;
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() min: number = 0;
  @Input() max: number = 999999;

  value: number | null = null;

  onChange = (value: any) => {};
  onTouched = () => {};

  constructor(@Self() @Optional() public ngControl: NgControl) {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  get shouldShowErrors(): boolean {
    if (!this.ngControl || !this.ngControl.control) {
      return false;
    }

    const control = this.ngControl.control;

    return control.invalid && (control.touched || control.dirty);
  }

  writeValue(value: any): void {
    this.value = value === undefined || value === '' ? null : value;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onModelChange(newValue: number | null) {
    this.value = newValue === null || newValue === undefined ? null : newValue;
    this.onChange(this.value);
  }
}
