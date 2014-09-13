Statua ListDelete_L(LinkList &L, int i,ElemType &e){
    //在带头结点的单链表L中，删除第i个结点，并由e返回其值
    p = L;j = 0;
    while(p->next && j<i-1){  //寻找第i个结点，并令p指向其前驱
       p = p->next; j++;
    }
    if(p->next == NULL)
       return ERROR;
    q = p->next;   //暂存要删除结点
    p->next = q->next;
    e = q->data; free(q);   //释放结点
    return OK;
}   //ListDelete_L
