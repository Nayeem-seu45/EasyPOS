import { Component, OnInit } from '@angular/core';
import { TreeDragDropService, TreeNode } from 'primeng/api';
import { AppMenusClient, TreeNodeModel, UpdateAppMenuOrderCommand } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-reorder-app-menus',
  templateUrl: './reorder-app-menus.component.html',
  styleUrl: './reorder-app-menus.component.scss',
  providers: [AppMenusClient, TreeDragDropService]

})
export class ReorderAppMenusComponent implements OnInit {
  appMenus!: TreeNodeModel[];
  updatedMenus!: TreeNodeModel[];

  constructor(public entityClient: AppMenusClient,
    private customDialogService: CustomDialogService,
    private toast: ToastService
  ) {

  }

  ngOnInit() {
    this.getMenuTree();
  }

  private getMenuTree() {
    this.entityClient.getMenuTreeSelect().subscribe({
      next: (treeNodes: TreeNodeModel[]) => {
        this.appMenus = treeNodes;
        console.log(this.appMenus);
      }, error: (error) => {
        console.log(error);
      }
    });
  }

  cancel() {
    this.customDialogService.closeLastDialog(false);
  }

  onNodeDrop(event: any) {
    console.log(event)
    const dragNode: TreeNodeModel = event.dragNode;
    const dropNode: TreeNodeModel = event.dropNode;
    const dragNodeCurrentIndex = event.index;

    // Clone appMenus without recursive parent-child references
    this.updatedMenus = this.cloneTreeNodes(this.appMenus);
    console.log(this.updatedMenus)
    
    // const draggedNode = this.updatedMenus.find(x => x.key === dragNode.key);
    // console.log(draggedNode)
  }

  saveChangesOrder() {
    const command = new UpdateAppMenuOrderCommand();
    command.reorderedAppMenus = this.updatedMenus;
    this.entityClient.reorderAppMenus(command).subscribe({
      next: () => {
        this.toast.updated();
      }, error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

// Clone appMenus and ensure parents have empty children arrays to prevent deep nesting and circular references
private cloneTreeNodes(nodes: TreeNodeModel[]): TreeNodeModel[] {
  return nodes.map(node => this.cloneNode(node, null));
}

// Recursive function to clone a node, with controlled reference in parent property
private cloneNode(node: TreeNodeModel, parentNode: TreeNodeModel | null): TreeNodeModel {
  // Create a shallow copy of the node
  const clonedNode = new TreeNodeModel({
    ...node,
    parent: parentNode ? new TreeNodeModel({ ...parentNode, children: [] }) : null, // Set parent without children
    children: [] // Temporarily empty children to avoid infinite recursion
  });

  // Recursively clone children
  if (node.children && node.children.length > 0) {
    clonedNode.children = node.children.map(child => this.cloneNode(child, clonedNode));
  }

  return clonedNode;
}



}
