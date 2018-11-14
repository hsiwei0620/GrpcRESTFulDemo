package main

import (
	// "errors"
	// "flag"
	"fmt"
	"log"
	"net"
	"net/http"
	_ "strconv"
	"sync"

	"github.com/grpc-ecosystem/grpc-gateway/runtime"
	// "go.uber.org/zap"
	"golang.org/x/net/context"
	"google.golang.org/grpc" 
 
	API "path/to/project/APISource"
	"path/to/project/protos/callAPIService"
)

// var (
// 	rtn callAPIService.Check_Material_Reply
// )

type server struct{}

//Check_Material_Send : check material information
func (m *server) Check_Material_Send(ctx context.Context, req *callAPIService.Check_Material_Request) (*callAPIService.Check_Material_Reply, error) {

	var rtn callAPIService.Check_Material_Reply

	if req.Eqptid == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入機台號！"
		return &rtn, nil
	}

	if req.Engtp == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入工程別！"
		return &rtn, nil
	}

	if req.Mtrlcnt == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入至少一筆材料！"
		return &rtn, nil
	}

	mAry, err := API.ScanCheck(
		req,
		*mesServer,
	)
	if err != nil {
		return nil, err
	}
	return &mAry, nil
}

//Load_Drug_Recipe : laod drug
func (m *server) Load_Recipe_Drug_Send(ctx context.Context, req *callAPIService.Load_Recipe_Drug_Request) (*callAPIService.Load_Recipe_Drug_Reply, error) {

	var rtn callAPIService.Load_Recipe_Drug_Reply

	if req.Bayid == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入區域識別碼"
		return &rtn, nil
	}

	if req.Prepstartdate == "" || req.Prependdate == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入起始或結束時間"
		return &rtn, nil
	}

	if req.Needorderflag == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入排產順序(Y/N)"
		return &rtn, nil
	}

	if req.Preptype == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入備料種類(DRUG/RCUT)"
		return &rtn, nil
	}

	mAry, err := API.LoadDrug(
		req,
		*mesServer,
	)
	if err != nil {
		return nil, err
	}
	return &mAry, nil
}

//Move_In_Finish_Drug 
func (m *server) Move_In_Finish_Drug_Send(ctx context.Context, req *callAPIService.Move_In_Finish_Drug_Request) (*callAPIService.Move_In_Finish_Drug_Reply, error) {
	
	var rtn callAPIService.Move_In_Finish_Drug_Reply
	
	if req.Mtrlproductid == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入Mtrlproductid"
		return &rtn, nil
	}

	if req.Lotid == "" {
		rtn.Rtncode = "0000001"
		rtn.Rtnmesg = "請輸入工單號碼"
		return &rtn, nil
	}


	mAry, err := API.MoveInFinishDrug(
		req,
		*mesServer,
	)
	if err != nil {
		return nil, err
	}
	return &mAry, nil
}

//RungRPC run start
func RungRPC(wg *sync.WaitGroup) error {

	defer wg.Done()
	fmt.Printf("running gRPC server\n")

	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", *gRPCport))
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
		return err
	}

	s := grpc.NewServer()
	mux := runtime.NewServeMux()
	callAPIService.RegisterAPI_ServiceServer(s, &server{})
	if err := s.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
		return err
	}

	return http.ListenAndServe(":8080", mux)
}
