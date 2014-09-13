Status ListInsert_Sq(SeqList *L, int i, ElemType e)
{
	//在顺序表L中第i个位置上插入新结点e，
   	//i的合法值为1<=i<=Length+1
   	if (i<1||i>L->length+1){           //注：这里位置是从1开始的
    	return ERROR;
   	}
   	else{
   		for(j=L->length-1;j>=i-1;--j){ //注：C语言中下标从0开始
			L->elem[j+1]=L->elem[j];   //插入位置及之后的元素右移
    	}
    	L->elem[i-1]=e;                //插入e
      	L->length++;                   //表长加 1
	}
	return OK;
}
