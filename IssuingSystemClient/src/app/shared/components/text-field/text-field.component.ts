import {Component, Input, Optional, Self} from '@angular/core';
import {InputTextModule} from 'primeng/inputtext';
import {CommonModule} from '@angular/common';
import {ControlValueAccessor, FormsModule, NgControl} from '@angular/forms';


@Component({
  selector: 'app-text-field',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
  ],
  templateUrl: './text-field.component.html',
  styleUrl: './text-field.component.css'
})
export class TextFieldComponent implements ControlValueAccessor {
  @Input() id: string = `text-field-${Math.random().toString(36).substring(2, 9)}`;
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: 'text' | 'password' | 'email' = 'text';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;

  value: string = '';

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

  onChange = (value: any) => {};
  onTouched = () => {};

  writeValue(value: any): void {
    this.value = value;
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

  onModelChange(event: any) {
    this.value = event;
    this.onChange(this.value);
  }
}
