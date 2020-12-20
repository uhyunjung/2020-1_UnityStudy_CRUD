export const transactionReducer = (state, action) =>{
    var list = JSON.parse(localStorage.getItem('transactions'))
    switch (action.type){
        case "INSERT":
            list.push(action.payload)
            localStorage.setItem('transactions',JSON.stringify(list))
            return {list, currentIndex:-1}

        case "UPDATE":
            list[state.currentIndex] = action.payload
            localStorage.setItem('transactions',JSON.stringify(list))
            return {list, currentIndex:-1}
    
        case "DELETE":
            list.slice(action.payload,1)
            localStorage.setItem('transactions',JSON.stringify(list))
            return {list, currentIndex:-1}

        case "UPDATE-":
            return {list, currentIndex:-1}

        default:
            return state
    }
}