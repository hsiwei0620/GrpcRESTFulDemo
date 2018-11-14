package APISource

import (
	"fmt"
	// "os"
	"strconv"
	// "sync"
	"time"

	TX "path/to/project/protos/MqGrpcProject"
	msmqSend "path/to/project/protos/MqGrpcProject"
	API "path/to/project/protos/callAPIService"
	msmqCon "path/to/project/connectionConfig"
	// "go.uber.org/zap"
)

var (
	lotid  string
	reFlag bool //判斷是否全部材料都檢核OK
)



// 確認工單材料與替代材料
func checkMaterail(txOray TX.APIMTLST_Reply, bom API.Check_Material_Reply, bomCnt int) (int, string) {

	rtnCode := 0
	rtnMesg := ""
	for i := 0; i < bomCnt; i++ {
		if txOray.GetOary()[0].GetMtrlproductid() == bom.GetOary()[i].GetMtrlproductid() {
			return 0, "" //找到符合工單材料 return
		}
	}
	if txOray.GetMascnt() != "" {
		msaCnt, _ := strconv.Atoi(txOray.GetMascnt())
		for i := 0; i < msaCnt; i++ {
			for j := 0; j < bomCnt; j++ {
				if txOray.GetOary2()[i].GetSubmtrlprodid() == bom.GetOary()[j].GetMtrlproductid() {
					return 0, "" //找到符合工單材料 return
				}
			}
		}
	}

	rtnCode = 1
	rtnMesg = " : 與工單材料不符\n"

	return rtnCode, rtnMesg
}

// Add return code/message array
func addRtncode(code string, mesg string) *API.RtnMtrlA {

	reFlag = false
	rtn := new(API.RtnMtrlA)
	rtn.Rtncode = code
	rtn.Rtnmesg = mesg

	return rtn
}

// ScanCheck Statr
func ScanCheck(req *API.Check_Material_Request, mesServer string) (API.Check_Material_Reply, error) {

	var reAry API.Check_Material_Reply  // 回傳Reply內容
	apmeqrsvOary := new(TX.APMEQRSVoA1) //暫存APMEQRSV oary
	nTime := time.Now()                 //取今天時間
	reBomCnt := 0                       //最後回傳的BOM筆數
	var allMtrlQty []float64            //暫存材料數量(最後確認總數用)
	reFlag = true                       //初始狀態

	conn, ctx, myCloseClient, err := msmqCon.Connection()
	if err != nil {
		return reAry, err
	}
	defer myCloseClient()

	client := msmqSend.NewMqGrpcsClient(conn)

	// 配藥切膠用 engtp : DRUG,RCUT 才執行
	if req.GetEngtp() == "DRUG" || req.GetEngtp() == "RCUT" {

		logDay := nTime.AddDate(0, 0, -3).Format("2006-01-02")
		fmt.Printf("day:" + logDay + "\n")

		// Send APLRSVPR Start

		APLRSVPR, err := client.APLRSVPR_Send(ctx, &msmqSend.APLRSVPR_Request{
			Serverip:   mesServer,
			Resveqptid: req.GetEqptid(),
			Resvdate:   logDay,
			Preptype:   req.GetEngtp(),
		})
		if err != nil {
			return reAry, err
		}

		// rtn_code !=0 時 return

		if rtncode, _ := strconv.Atoi(APLRSVPR.GetRtncode()); rtncode > 0 {
			reAry.Rtncode = APLRSVPR.GetRtncode()
			reAry.Rtnmesg = "APLRSVPR:" + APLRSVPR.GetRtnmesg()
			return reAry, nil
		}
		inLotCnt, _ := strconv.Atoi(APLRSVPR.GetLotarycnt())
		if inLotCnt == 0 {
			reAry.Rtncode = "2"
			reAry.Rtnmesg = "無可配藥工單！"
			return reAry, nil
		}

		drCnt := 0
		for i := 0; i < inLotCnt; i++ {
			if APLRSVPR.Oary[i].GetPrepstat() == "INIT" {
				drCnt++
				apmeqrsvOary.Productid = APLRSVPR.Oary[i].GetProductid()
				apmeqrsvOary.Routeid = APLRSVPR.Oary[i].GetMainrouteid()
				apmeqrsvOary.Nxopeno = APLRSVPR.Oary[i].GetNxopeno()
				apmeqrsvOary.Routever = APLRSVPR.Oary[i].GetMainroutever()
				apmeqrsvOary.Nxopeid = APLRSVPR.Oary[i].GetNxopeid()
				apmeqrsvOary.Nxopever = APLRSVPR.Oary[i].GetNxopever()
				apmeqrsvOary.Resveqptid = APLRSVPR.Oary[i].GetResveqptid()
				apmeqrsvOary.Lotid = APLRSVPR.Oary[i].GetLotid()
				break
			}
		}
		if drCnt == 0 {
			reAry.Rtncode = "128"
			reAry.Rtnmesg = "無可配藥工單！"
			return reAry, nil
		}

	} else {
		// Send APMEQRSV Start

		if req.GetLotid() != "" { //有無指定工單
			lotid = req.GetLotid()
		} else {
			lotid = ""
		}

		APMEQRSV, err := client.APMEQRSV_Send(ctx, &msmqSend.APMEQRSV_Request{
			Serverip:    mesServer,
			Actiontype:  "Q",
			Resveqptid:  req.GetEqptid(),
			Lotid:       lotid,
			Onlyresvflg: "Y",
		})
		if err != nil {
			return reAry, err
		}

		// rtn_code !=0 時 return
		if rtncode, _ := strconv.Atoi(APMEQRSV.GetRtncode()); rtncode > 0 {
			reAry.Rtncode = APMEQRSV.GetRtncode()
			reAry.Rtnmesg = "APMEQRSV:" + APMEQRSV.GetRtnmesg()
			return reAry, nil
		} else if inLotCnt, _ := strconv.Atoi(APMEQRSV.GetLotarycnt()); inLotCnt == 0 {
			reAry.Rtncode = "2"
			reAry.Rtnmesg = "無排程工單！"
			return reAry, nil
		}

		apmeqrsvOary.Productid = APMEQRSV.Oary[0].GetProductid()
		apmeqrsvOary.Routeid = APMEQRSV.Oary[0].GetRouteid()
		apmeqrsvOary.Nxopeno = APMEQRSV.Oary[0].GetNxopeno()
		apmeqrsvOary.Routever = APMEQRSV.Oary[0].GetRoutever()
		apmeqrsvOary.Nxopeid = APMEQRSV.Oary[0].GetNxopeid()
		apmeqrsvOary.Nxopever = APMEQRSV.Oary[0].GetNxopever()
		apmeqrsvOary.Resveqptid = APMEQRSV.Oary[0].GetResveqptid()
		apmeqrsvOary.Lotid = APMEQRSV.Oary[0].GetLotid()

	}

	// Send APLPRDBM Start
	// 有傳入上一次紀錄的BOM資訊不用再發一次
	if inBomCnt, _ := strconv.Atoi(req.GetBomcnt()); inBomCnt > 0 {

		reAry.Bomcnt = req.GetBomcnt()

		for i := 0; i < inBomCnt; i++ {
			bom := new(API.BomA)
			bom = req.GetIary2()[i]
			// bom.Mtrlproductid = req.GetIary2()[i].GetMtrlproductid()
			// bom.Mtrlproductdsc = req.GetIary2()[i].GetMtrlproductdsc()
			// bom.Mtrlcate = req.GetIary2()[i].GetMtrlcate()
			// bom.Planqty = req.GetIary2()[i].GetPlanqty()
			// bom.Mtrlunit = req.GetIary2()[i].GetMtrlunit()
			// bom.Spcfymtrllotid = req.GetIary2()[i].GetSpcfymtrllotid()
			// bom.Ext1 = req.GetIary2()[i].GetExt1()
			// bom.Ext2 = req.GetIary2()[i].GetExt2()
			// bom.Ext3 = req.GetIary2()[i].GetExt3()
			// bom.Ext4 = req.GetIary2()[i].GetExt4()
			// bom.Ext5 = req.GetIary2()[i].GetExt5()
			// bom.Parentid = req.GetIary2()[i].GetParentid()
			reAry.Oary = append(reAry.Oary, bom)
		}
	} else {
		APLPRDBM, err := client.APLPRDBM_Send(ctx, &msmqSend.APLPRDBM_Request{
			Serverip:   mesServer,
			Productid:  apmeqrsvOary.Productid,
			Eccode:     "00",
			Routeid:    apmeqrsvOary.Routeid,
			Nxopeno:    apmeqrsvOary.Nxopeno,
			Routever:   apmeqrsvOary.Routever,
			Opeid:      apmeqrsvOary.Nxopeid,
			Opever:     apmeqrsvOary.Nxopever,
			Resveqptid: apmeqrsvOary.Resveqptid,
			Lotid:      apmeqrsvOary.Lotid,
		})
		if err != nil {
			return reAry, err
		}

		// rtn_code !=0 時 return
		if rtncode, _ := strconv.Atoi(APLPRDBM.GetRtncode()); rtncode > 0 {
			reAry.Rtncode = APLPRDBM.GetRtncode()
			reAry.Rtnmesg = "APLPRDBM:" + APLPRDBM.GetRtnmesg()
			return reAry, nil
		} else if bomcnt, _ := strconv.Atoi(APLPRDBM.GetOary1()[1].GetMtrlcnt()); bomcnt == 0 {
			reAry.Rtncode = "16"
			reAry.Rtnmesg = "找不到材料明細"
			return reAry, nil
		}

		bomCnt, _ := strconv.Atoi(APLPRDBM.GetOary1()[1].GetMtrlcnt())

		// 配藥先確認藥包有沒有在BOM裡面
		if req.GetProductid() != "" && req.GetEngtp() == "DRUG" {
			checkCnt := 0
			for i := 0; i < bomCnt; i++ {
				if req.GetProductid() == APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlproductid() {
					checkCnt++
				}
			}
			if checkCnt == 0 {
				reAry.Rtncode = "16"
				reAry.Rtnmesg = "找不到配藥明細"
				return reAry, nil
			}
		}

		if req.GetProductid() != "" && req.GetEngtp() == "DRUG" { // 配藥只需傳回藥包的資訊
			for i := 0; i < bomCnt; i++ {
				if req.GetProductid() == APLPRDBM.GetOary1()[1].GetOary2()[i].GetParentid() {
					bom := new(API.BomA)
					bom.Mtrlproductid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlproductid()
					bom.Mtrlproductdsc = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlproductdsc()
					bom.Mtrlcate = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlcate()
					bom.Planqty = APLPRDBM.GetOary1()[1].GetOary2()[i].GetPlanqty()
					bom.Mtrlunit = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlunit()
					bom.Spcfymtrllotid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetSpcfymtrllotid()
					bom.Ext1 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_1()
					bom.Ext2 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_2()
					bom.Ext3 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_3()
					bom.Ext4 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_4()
					bom.Ext5 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_5()
					bom.Parentid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetParentid()
					reAry.Oary = append(reAry.Oary, bom)
					reBomCnt++
				}
			}
		} else {
			for i := 0; i < bomCnt; i++ {
				bom := new(API.BomA)
				bom.Mtrlproductid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlproductid()
				bom.Mtrlproductdsc = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlproductdsc()
				bom.Mtrlcate = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlcate()
				bom.Planqty = APLPRDBM.GetOary1()[1].GetOary2()[i].GetPlanqty()
				bom.Mtrlunit = APLPRDBM.GetOary1()[1].GetOary2()[i].GetMtrlunit()
				bom.Spcfymtrllotid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetSpcfymtrllotid()
				bom.Ext1 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_1()
				bom.Ext2 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_2()
				bom.Ext3 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_3()
				bom.Ext4 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_4()
				bom.Ext5 = APLPRDBM.GetOary1()[1].GetOary2()[i].GetExt_5()
				bom.Parentid = APLPRDBM.GetOary1()[1].GetOary2()[i].GetParentid()
				reAry.Oary = append(reAry.Oary, bom)
				reBomCnt++
			}
		}
		reAry.Bomcnt = strconv.Itoa(reBomCnt) //回傳筆數
		//設定全部材料的總數量結構預設為0
		for i := 0; i < reBomCnt; i++ {
			allMtrlQty = append(allMtrlQty, 0)
		}
	}

	// 確認輸入的材料包

	mtrlCnt, _ := strconv.Atoi(req.GetMtrlcnt())

	for i := 0; i < mtrlCnt; i++ {
		APIMTLST, err := client.APIMTLST_Send(ctx, &msmqSend.APIMTLST_Request{
			Serverip:      mesServer,
			Mtrlproductid: req.GetIary()[i].GetMtrlproductid(),
			Mtrllotid:     req.GetIary()[i].GetBarcode(),
			Querysubmtrl:  "Y",
		})
		if err != nil {
			return reAry, err
		}
		if rtncode, _ := strconv.Atoi(APIMTLST.GetRtncode()); rtncode > 0 {
			reAry.Rtncode = APIMTLST.GetRtncode()
			reAry.Rtnmesg = "APIMTLST:" + APIMTLST.GetRtnmesg()
			return reAry, nil
		}
		if mtrlCnt, _ := strconv.Atoi(APIMTLST.GetMtrlcnt()); mtrlCnt == 0 {
			rtn := addRtncode("1", req.GetIary()[i].GetBarcode()+" : 條碼不存在")
			reAry.Oary2 = append(reAry.Oary2, rtn)
			continue
		}

		// 判斷、計算重複掃描條碼
		repeatCnt := 0
		for j := 0; j < mtrlCnt; j++ {
			if req.GetIary()[i].GetBarcode() == req.GetIary()[j].GetBarcode() {
				repeatCnt++
			}
		}
		if repeatCnt > 1 {
			rtn := addRtncode("2", req.GetIary()[i].GetBarcode()+" : 重覆刷入條碼")
			reAry.Oary2 = append(reAry.Oary2, rtn)
			continue
		}

		checkCode, checkMesg := checkMaterail(*APIMTLST, reAry, reBomCnt)
		if checkCode > 0 {
			rtn := addRtncode("4", req.GetIary()[i].GetBarcode()+checkMesg) // 材料與工單不同
			reAry.Oary2 = append(reAry.Oary2, rtn)
		}

		if APIMTLST.GetOary()[0].GetMtrlstat() != "AVAL" {
			rtn := addRtncode("8", req.GetIary()[i].GetBarcode()+" : 材料不合格") // 材料與工單不同
			reAry.Oary2 = append(reAry.Oary2, rtn)
		}

		if timeSub(time.Now().Format("20060102"), APIMTLST.GetOary()[0].GetExpiredate()) > 0 {
			rtn := addRtncode("16", req.GetIary()[i].GetBarcode()+" : 材料已過期") // 材料與工單不同
			reAry.Oary2 = append(reAry.Oary2, rtn)
		}

		if mtrlQty, _ := strconv.Atoi(APIMTLST.GetOary()[0].GetMtrlqty()); mtrlQty <= 0 {
			rtn := addRtncode("999", req.GetIary()[i].GetBarcode()+" : 材料數量小於0") // 材料與工單不同
			reAry.Oary2 = append(reAry.Oary2, rtn)
		}

		for j := 0; j < reBomCnt; j++ {
			if req.GetIary()[i].GetMtrlproductid() == reAry.GetOary()[j].GetMtrlproductid() {
				useQty, _ := strconv.ParseFloat(req.GetIary()[i].GetMtrlproductid(), 64)
				allMtrlQty[j] += useQty
			}
		}

	}

	// 配藥確認是否全部材料數量都夠
	if req.GetEngtp() == "DRUG" {
		for i := 0; i < reBomCnt; i++ {
			if planQty, _ := strconv.ParseFloat(reAry.GetOary()[i].GetPlanqty(), 64); planQty > allMtrlQty[i] {
				reAry.Rtncode = "64"
				reAry.Rtnmesg = reAry.GetOary()[i].GetMtrlproductid() + " : 掃入材料不足"
				return reAry, nil
			}
		}
	}

	// 都沒問題回傳rtncode回傳0,mesg回傳空白
	if reFlag {
		reAry.Rtncode = "1111111"
		reAry.Rtnmesg = "材料檢核有誤"
	} else {
		reAry.Rtncode = "0000000"
		reAry.Rtnmesg = ""
	}

	return reAry, nil
}
