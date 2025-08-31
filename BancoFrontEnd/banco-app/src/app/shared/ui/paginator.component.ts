import { Component, EventEmitter, Input, Output } from '@angular/core';
@Component({
  selector:'app-paginator',
  template:`<div class="p">
    <button class="btn" [disabled]="page<=1" (click)="go(page-1)">&lt;</button>
    <span>{{page}} / {{pages}}</span>
    <button class="btn" [disabled]="page>=pages" (click)="go(page+1)">&gt;</button>
  </div>`,
  styles:[`.p{display:flex;gap:8px;align-items:center;padding:10px}`]
})
export class PaginatorComponent{
  @Input() page=1; @Input() total=0; @Input() size=10;
  @Output() change = new EventEmitter<number>();
  get pages(){ return Math.max(1, Math.ceil(this.total/this.size)); }
  go(p:number){ this.change.emit(p); }
}
