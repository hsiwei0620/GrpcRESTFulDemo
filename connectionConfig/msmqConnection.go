package connectionConfig

import (
	"fmt"
	"time"

	"golang.org/x/net/context"
	"google.golang.org/grpc"
)

//ClientCloseFunc 關閉client
type ClientCloseFunc func()

//Connection for msmq config
func Connection() (*grpc.ClientConn, context.Context, ClientCloseFunc, error) {
	conn, err := grpc.Dial("192.1.1.61:8080", grpc.WithInsecure())
	if err != nil {
		fmt.Printf("Connection failed: %v", err)
		return nil, nil, nil, err
	}

	ctx, cancel := context.WithTimeout(context.Background(), 30000*time.Millisecond)

	var myCloseClient ClientCloseFunc = func() {
		conn.Close()
		cancel()
	}

	return conn, ctx, myCloseClient, nil

}
