import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, Observer } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit
{
  title: string = 'Client';
  users: any;

  constructor(private http: HttpClient)
  {

  }

  public ngOnInit(): void
  {
    this.http.get("https://localhost:5001/api/users").subscribe(this.newMethod());
  }

  private newMethod(): Partial<Observer<Object>>
  {
    return{
      next: responce => this.users = responce,
      error: error => console.log(error),
      complete: () => console.log("Request has completed")
    }
  }
}
