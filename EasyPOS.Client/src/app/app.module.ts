import { NgModule } from '@angular/core';
import { HashLocationStrategy, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AppLayoutModule } from './layout/app.layout.module';
import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { IconService } from './demo/service/icon.service';
import { PhotoService } from './demo/service/photo.service';
import { CoreModule } from './core/core.module';
import { ConditionalDraggableDirective } from './shared/direvtives/conditional-draggable.directive';

@NgModule({
    declarations: [
        AppComponent, 
        NotfoundComponent,
        ConditionalDraggableDirective
    ],
    imports: [
        AppRoutingModule, 
        AppLayoutModule,
        CoreModule,
    ],
    providers: [
        { provide: LocationStrategy, useClass: PathLocationStrategy },
        IconService,
        PhotoService,
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
