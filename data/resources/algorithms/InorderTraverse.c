Status InorderTraverse(BiTree T,Status(*Visit)(TElemType e)){
	//中序遍历二叉树的非递归算法，对每个元素调用函数Visit
	InitStack(S);  p = T;
	while(p || !StackEmpty(S)){
		if(p){
			Push(S,p);
			p = p->lchild;
		}
		else{
			Pop(S,p);
			if(!Visit(p->data)){
				return ERROR;
			}
			p = p->rchild;
		}
	}//while
	return OK;
}  //InorderTraverse
