import { LoginComponent } from './login/login.component';
import { AuthGuardService } from './authentificationService/auth-guard.service';
import { Routes } from "@angular/router";
import { ExperimentsOverviewComponent } from "./experiments-overview/experiments-overview.component";

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'experiments', component: ExperimentsOverviewComponent, canActivate: [AuthGuardService] },
];

export {routes}
