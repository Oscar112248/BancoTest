import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Output() search = new EventEmitter<string>();

  emitSearch(event: Event) {
  const value = (event.target as HTMLInputElement).value;
  this.search.emit(value);
}
}
