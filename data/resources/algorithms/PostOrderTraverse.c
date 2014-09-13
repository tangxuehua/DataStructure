Status PostOrderTraverse(BiTree T,Status(*Visit)(TElemType e)){
    //后序遍历二叉树的非递归算法，对每个元素调用函数Visit
    BiTree p = T,q = NULL;
    SqStack S;	InitStack(S);  Push(S,p);
    while (!StackEmpty(S)){
		if(p && p != q){
	    	Push(S,p);
	    	p=p->lchild;
		}
    	else{
	    	Pop(S,p);
	    	if(!StackEmpty(S)){
				if(p->rchild && p->rchild != q){
		    		Push(S,p);
            	    p=p->rchild;}  //if
				else{			
           	   		Visit(p->data);
		    		q = p;}  //else
	    	} //if
		}  //else
    }  //while
    return OK;
}
