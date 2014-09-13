int BinSearch(SSTable R[],int n,KeyType k){
	//在有序表R[1..n]中折半查找关键字等于k的记录
	//若找到,则返回该记录在表中的位置;否则返回0
	int low = 1,high = n,mid;
	while(low <= high){
		mid = (low + high) / 2;
		if(R[mid].key == k){
			return mid;
		}
		else if(R[mid].key < k){
			low = mid + 1;
		}
		else{
			high = mid - 1;
		}
	}
	return 0;
}	//BinSearch
