Statua ListInsert_L(LinkList &L, int i,ElemType e){
    //在带头结点的单链表L中，在第i个位置之前插入值为e的一个结点
    p = L;j = 0;
    while(p && j<i-1){  //寻找第i个结点，并令p指向其前驱
       p = p->next; j++;
    }
    if(p == NULL)
       return ERROR;
    s = (LinkList)malloc(sizeof(LNode)); //开辟一个新结点
    s->data = e;
    s->next = p->next;
    p->next = s;
    return OK;
}   //ListDelete_L
