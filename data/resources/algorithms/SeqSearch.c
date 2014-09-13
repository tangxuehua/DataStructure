int SeqSearch(SSTable R[],int n,KeyType k){
	//在顺序表R[1..n]中,顺序查找关键字等于k的记录
	//若查找成功,则返回该记录在表中的位置;否则返回0
	int i;
	R[0].key = k;  //将R[0]作为哨兵
	i = n;         //从第n个位置起向前扫描
	while(R[i].key != k){
		i--;
	}
	return i;
}   //SeqSearch
