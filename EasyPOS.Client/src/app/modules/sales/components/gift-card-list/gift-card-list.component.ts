import { Component, inject } from '@angular/core';
import { GiftCardsClient } from 'src/app/modules/generated-clients/api-service';
import { GiftCardDetailComponent } from '../gift-card-detail/gift-card-detail.component';

@Component({
  selector: 'app-gift-card-list',
  templateUrl: './gift-card-list.component.html',
  styleUrl: './gift-card-list.component.scss',
  providers: [GiftCardsClient]
})
export class GiftCardListComponent {
  detailComponent = GiftCardDetailComponent;
  pageId = '061b772d-cb21-4954-0591-08dcf84dadd6'

  entityClient: GiftCardsClient = inject(GiftCardsClient);
}
