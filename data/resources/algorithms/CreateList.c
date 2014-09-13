 Void CreateList_l(LinkList &L, int n){
    //逆位序输入n个结点的值，建立带头结点的单链线性表L
    //所谓逆位序是指每次输入的结点都插到链表中第一个结点之前
    L=(LinkList)malloc(sizeof(Lnode));  L->next=NULL;//头结点初始化
    for(i=n;i>0;i--){
       p=(LinkList)malloc(sizeof(Lnode));  scanf(&p->data); //生成新结点     
       p->next=L->next;   //将新结点插入到第一个结点之前
       L->next=p;
    }
    return;
 }  //CreateList_L
