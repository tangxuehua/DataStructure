BSTree BSTSearch(BSTree BST,KeyType k){
	BSTree p;
	p = BST;
	while(p != NULL && p->key != k){
		if(k < p->key){
			p = p->lchild;  //ËÑË÷×ó×ÓÊ÷
		}
		else{
			p = p->rchild;  //ËÑË÷ÓÒ×ÓÊ÷
		}
	}
	return p;
}  //BinSearch
