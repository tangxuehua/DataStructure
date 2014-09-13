Status ListDelete_Sq(SqList &L, int i, ElemType &e)
  //在顺序线性表L中删除第i个元素,i的合法值为1<=i<=L.length
{
   If(i<1||i>L.length){
       Return ERROR;   //i的值不合法
   }
   e=L.elem[i-1];     //被删除元素用e保存
   for(j=i;j<=L.length-1;j++) {
		L.elem[j-1]=L.elem[j];   //被删除元素之后的元素左移
   }
   L.length--;    //表长减1   
   Return OK;
} //ListDelete_Sq
