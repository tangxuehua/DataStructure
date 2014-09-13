void SelectSort(SSTable R[],int n){
	int i,j,k;
	for(i = 1;i < n;i++){
		k = i;
		for(j = i + 1;j <= n;j++){
			if(R[j].key < R[k].key){
				k = j;
			}
		}
		if(i != k){     //½»»»R[i]ºÍR[k]
			R[0] = R[k];
			R[k] = R[i];
			R[i] = R[0];
		}
	}
	return;
}
